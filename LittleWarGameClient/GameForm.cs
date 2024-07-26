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

namespace LittleWarGameClient
{
    internal partial class GameForm : Form
    {
        private static GameForm? thisForm;
        internal static string baseUrl = @"https://littlewargame.com/play";
        private readonly Settings settings;
        private readonly Fullscreen fullScreen;
        private readonly KeyboardHandler kbHandler;
        private readonly VersionHandler vHandler;
        private readonly AudioManager audioMngr;
        private readonly Form overlayForm;

        private bool wasSmallWindow = false;
        private bool gameHasLoaded = false;
        private bool mouseLocked;

        public GameForm(Form overlayForm)
        {
            thisForm = this;
            this.overlayForm = overlayForm;
            InitializeComponent();
            audioMngr = new AudioManager(Text);
            settings = new Settings();
            fullScreen = new Fullscreen(this, settings);
            kbHandler = new KeyboardHandler(fullScreen, settings);
            vHandler = new VersionHandler(settings);
            Size = settings.GetWindowSize();
            mouseLocked = settings.GetMouseLock();
            InitWebView();
        }

        private void InitWebView()
        {
            var path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            var cefSettings = new CefSettings();
            cefSettings.RootCachePath = Path.Join(path, "data");
            Cef.Initialize(cefSettings);
            webView.KeyboardHandler = kbHandler;
            webView.RequestHandler = new RequestInterceptor();
            webView.LoadUrl(baseUrl);
            loadingPanel.SetDoubleBuffered();
            loadingPanel.BringToFront();
            //Path.Join(path, "downloads");
            //webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            //webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = true;
            //webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        private void CaptureCursor()
        {
            if (mouseLocked)
            {
                webView.Capture = true;
                var webViewBounds = new Rectangle(webView.PointToScreen(Point.Empty), webView.Size);
                Cursor.Clip = webViewBounds;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                webView.Capture = false;
                Cursor.Clip = Rectangle.Empty;
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            CaptureCursor();
            ResizeGameWindows();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            CaptureCursor();
            settings.SetWindowSize(Size);
            settings.SaveAsync();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            overlayForm.Size = webView.Size;
            CaptureCursor();
            ResizeGameWindows();
        }

        private void ResizeGameWindows()
        {
            if (gameHasLoaded)
            {
                if (Height <= 800 && !wasSmallWindow)
                {
                    wasSmallWindow = true;
                    ElementMessage.CallJSFunc(webView, "setSmallWindowSizes");
                }
                else if (Height > 800 && wasSmallWindow)
                {
                    wasSmallWindow = false;
                    ElementMessage.CallJSFunc(webView, "setNormalWindowSizes");
                }
            }
        }

        private void ForceResizeGameWindows()
        {
            if (Height <= 800)
            {
                wasSmallWindow = true;
                ElementMessage.CallJSFunc(webView, "setSmallWindowSizes");
            }
            else if (Height > 800)
            {
                wasSmallWindow = false;
                ElementMessage.CallJSFunc(webView, "setNormalWindowSizes");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            audioMngr.DestroySession();
            OverlayForm.InvokeUI(() =>
            {
                overlayForm.Close();
            });
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
            var webViewBounds = new Rectangle(webView.PointToScreen(Point.Empty), webView.Size);
            overlayForm.Location = webViewBounds.Location;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            overlayForm.Size = webView.Size;
            var webViewBounds = new Rectangle(webView.PointToScreen(Point.Empty), webView.Size);
            overlayForm.Location = webViewBounds.Location;
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
                webView.ExecuteScriptAsync(addonJS);
                ElementMessage.CallJSFunc(webView, "init.function", $"\"{vHandler.CurrentVersion}\", {settings.GetMouseLock().ToString().ToLower()}, {settings.GetVolume()}");
                kbHandler.InitHotkeyNames((ChromiumWebBrowser)sender, settings);
            }
        }

        private void webView_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {
            ElementMessage? msg = JsonSerializer.Deserialize<ElementMessage>((string)e.Message);
            if (msg != null)
            {
                switch (msg.Type)
                {
                    case ButtonType.FullScreen:
                        InvokeUI(() =>
                        {
                            fullScreen.Toggle();
                        });
                        break;
                    case ButtonType.Exit:
                        Application.Exit();
                        break;
                    case ButtonType.MouseLock:
                        if (msg.Value != null && bool.Parse(msg.Value) == true)
                            mouseLocked = true;
                        else
                            mouseLocked = false;
                        settings.SetMouseLock(mouseLocked);
                        settings.SaveAsync();
                        InvokeUI(() =>
                        {
                            CaptureCursor();
                        });
                        break;
                    case ButtonType.InitComplete:
                        gameHasLoaded = true;
                        ForceResizeGameWindows();
                        InvokeUI(() =>
                        {
                            loadingPanel.Visible = false;
                            loadingTimer.Enabled = false;
                            loadingText.Text = "Reconnecting";
                        });
                        break;
                    case ButtonType.VolumeChanging:
                        if (msg.Value != null)
                        {
                            var val = float.Parse(msg.Value);
                            audioMngr.ChangeVolume(val);
                        }
                        break;
                    case ButtonType.VolumeChanged:
                        if (msg.Value != null)
                        {
                            var val = float.Parse(msg.Value);
                            audioMngr.ChangeVolume(val);
                            settings.SetVolume(val);
                            settings.SaveAsync();
                        }
                        break;
                }
            }
        }

        internal static void InvokeUI(Action a)
        {
            if (thisForm != null)
                thisForm.BeginInvoke(new MethodInvoker(a));
        }
    }
}
