using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using NAudio.CoreAudioApi;
using nud2dlib.Windows.Forms;
using nud2dlib;
using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using CefSharp.DevTools.Debugger;

namespace LittleWarGameClient
{
    internal partial class GameForm : Form
    {
        private static GameForm? formInstance;
        internal static GameForm Instance
        {
            get
            {
                if (formInstance == null || formInstance.IsDisposed)
                    formInstance = new GameForm();
                return formInstance;
            }
        }

        internal const string baseUrl = @"https://littlewargame.com/play";
        private readonly Settings settings;
        private readonly KeyboardHandler kbHandler;
        private readonly VersionHandler vHandler;
        private readonly AudioManager audioMngr;
        private FormWindowState PreviousWindowState;

        private bool wasSmallWindow = false;
        private bool gameHasLoaded = false;
        private bool mouseLocked;

        internal GameForm()
        {
            PreInitWeb();
            InitializeComponent();
            settings = new Settings();
            audioMngr = new AudioManager(Text);
            kbHandler = new KeyboardHandler(settings);
            vHandler = new VersionHandler(settings);
            InitScreen();
            InitWebView();
        }

        private void PreInitWeb()
        {
            var path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            var cefSettings = new CefSettings();
            cefSettings.CefCommandLineArgs.Add("no-proxy-server", "1");
            cefSettings.CefCommandLineArgs.Add("disable-plugins-discovery", "1");
            cefSettings.CefCommandLineArgs.Add("disable-extensions", "1");
            cefSettings.RootCachePath = Path.Join(path, "data");
            Cef.Initialize(cefSettings);
        }

        private void InitWebView()
        {
            webBrowser.JavascriptMessageReceived += ElementMessage.JSMessageReceived;
            webBrowser.KeyboardHandler = kbHandler;
            webBrowser.RequestHandler = new RequestInterceptor();
            webBrowser.DownloadHandler = new DownloadInterceptor();
            webBrowser.LoadUrl(baseUrl);
            loadingPanel.SetDoubleBuffered();
            loadingPanel.BringToFront();
        }

        private void InitScreen()
        {
            Size = settings.GetWindowSize();
            mouseLocked = settings.GetMouseLock();
            PreviousWindowState = WindowState;
            if (settings.GetFullScreen())
                EnterFullscreen();
            else
                LeaveFullscreen();
        }

        internal void ToggleFullscreen()
        {
            bool state;
            if (WindowState == FormWindowState.Maximized && FormBorderStyle == FormBorderStyle.None)
            {
                state = false;
                LeaveFullscreen();
            }
            else
            {
                state = true;
                EnterFullscreen();
            }
            settings.SetFullScreen(state);
            settings.SaveAsync();
        }

        private void EnterFullscreen()
        {
            if (WindowState != FormWindowState.Maximized || FormBorderStyle != FormBorderStyle.None)
            {
                PreviousWindowState = WindowState;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
        }

        private void LeaveFullscreen()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = PreviousWindowState;
        }

        private void CaptureCursor()
        {
            if (mouseLocked)
            {
                webBrowser.Capture = true;
                var webViewBounds = new Rectangle(webBrowser.PointToScreen(Point.Empty), webBrowser.Size);
                Cursor.Clip = webViewBounds;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                webBrowser.Capture = false;
                Cursor.Clip = Rectangle.Empty;
            }
        }

        private void ResizeGameWindows()
        {
            if (gameHasLoaded)
            {
                if (Height <= 800 && !wasSmallWindow)
                {
                    wasSmallWindow = true;
                    ElementMessage.CallJSFunc(webBrowser, "setSmallWindowSizes");
                }
                else if (Height > 800 && wasSmallWindow)
                {
                    wasSmallWindow = false;
                    ElementMessage.CallJSFunc(webBrowser, "setNormalWindowSizes");
                }
            }
        }

        private void ForceResizeGameWindows()
        {
            if (Height <= 800)
            {
                wasSmallWindow = true;
                ElementMessage.CallJSFunc(webBrowser, "setSmallWindowSizes");
            }
            else if (Height > 800)
            {
                wasSmallWindow = false;
                ElementMessage.CallJSFunc(webBrowser, "setNormalWindowSizes");
            }
        }

        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            if (loadingText.Visible)
                loadingText.Visible = false;
            else
                loadingText.Visible = true;
        }

        private void GameForm_LocationChanged(object sender, EventArgs e)
        {
            var webViewBounds = new Rectangle(webBrowser.PointToScreen(Point.Empty), webBrowser.Size);
            OverlayForm.Instance.Location = webViewBounds.Location;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            OverlayForm.Instance.Size = webBrowser.Size;
            var webViewBounds = new Rectangle(webBrowser.PointToScreen(Point.Empty), webBrowser.Size);
            OverlayForm.Instance.Location = webViewBounds.Location;
            OverlayForm.Instance.AddOverlayMessage($"InitDone", new Notification("Overlay Initialized"));
        }

        private void webView_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                InvokeUI(() =>
                {
                    loadingPanel.Visible = true;
                    loadingTimer.Enabled = true;
                    gameHasLoaded = false;
                });
            }
            else // has loaded
            {
                var addonJS = System.IO.File.ReadAllText("js/addons.js");
                webBrowser.ExecuteScriptAsync(addonJS);
                ElementMessage.CallJSFunc(webBrowser, "init.function", $"\"{vHandler.CurrentVersion}\", {settings.GetMouseLock().ToString().ToLower()}, {settings.GetVolume()}");
                kbHandler.InitHotkeyNames((ChromiumWebBrowser)sender, settings);
            }
        }

        internal void InvokeUI(Action a)
        {
            if (formInstance != null && formInstance.InvokeRequired)
            {
                if (formInstance.IsHandleCreated)
                    formInstance.BeginInvoke(new MethodInvoker(a));
            }
            else
            {
                a.Invoke();
            }
        }

        private void GameForm_Deactivate(object sender, EventArgs e)
        {
            if (!OverlayForm.Instance.IsActivated)
                OverlayForm.Instance.Visible = false;
        }

        private void GameForm_Activated(object sender, EventArgs e)
        {
            CaptureCursor();
            ResizeGameWindows();
            if (!OverlayForm.Instance.IsDisposed)
                OverlayForm.Instance.Visible = true;
            SendKeys.Send("%{F16}"); //Alt-Tab fix for game
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.None:
                    e.Cancel = true;
                    break;
                case CloseReason.UserClosing:
                    audioMngr.DestroySession();
                    webBrowser.Dispose();
                    Application.Exit();
                    break;
            }
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            OverlayForm.Instance.Size = webBrowser.Size;
            CaptureCursor();
            ResizeGameWindows();
        }

        private void GameForm_ResizeEnd(object sender, EventArgs e)
        {
            CaptureCursor();
            settings.SetWindowSize(Size);
            settings.SaveAsync();
        }

        internal void MouseLock(bool choice)
        {
            mouseLocked = choice;
            settings.SetMouseLock(mouseLocked);
            settings.SaveAsync();
            CaptureCursor();
        }

        internal void AddonsLoadedPostLogic()
        {
            gameHasLoaded = true;
            ForceResizeGameWindows();
            loadingPanel.Visible = false;
            loadingTimer.Enabled = false;
            loadingText.Text = "Reconnecting";
        }

        internal void ChangeVolume(float value)
        {
            audioMngr.ChangeVolume(value);
        }

        internal void VolumeChangePostLogic(float value)
        {
            audioMngr.ChangeVolume(value);
            settings.SetVolume(value);
            settings.SaveAsync();
        }
    }
}
