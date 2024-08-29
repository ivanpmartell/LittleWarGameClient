using CefSharp;
using CefSharp.WinForms;
using LittleWarGameClient.Handlers;
using LittleWarGameClient.Helpers;
using LittleWarGameClient.Interceptors;

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
#pragma warning disable CS8618
        internal static string InstanceName { get; set; }
#pragma warning restore CS8618

        internal const string baseUrl = @"https://littlewargame.com/play";
        private readonly SettingsHandler settings;
        private readonly KeyboardHandler kbHandler;
        private readonly VersionHandler versionHandler;
        private readonly AudioHandler audioHandler;
        private FormWindowState PreviousWindowState;

        internal int requestCallCounter = 0;
        private int requestCallWhereLoadingFinished = -1;
        private bool wasSmallWindow = false;
        private bool gameHasLoaded = false;
        private bool mouseLocked;

        internal GameForm()
        {
            if (InstanceName == null)
                throw new MissingFieldException(nameof(InstanceName));
            PreInitWeb();
            InitializeComponent();
            Text = $"Littlewargame({InstanceName})";
            loadingText.Font = new Font(FontHandler.lwgFont, 48F, FontStyle.Regular, GraphicsUnit.Point);
            settings = new SettingsHandler();
            audioHandler = new AudioHandler(Text);
            kbHandler = new KeyboardHandler(settings);
            versionHandler = new VersionHandler(settings);
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
            cefSettings.RootCachePath = Path.Join(path, "data", InstanceName);
            Cef.Initialize(cefSettings);
        }

        private void InitWebView()
        {
            webBrowser.JavascriptMessageReceived += ElementMessage.JSMessageReceived;
            webBrowser.KeyboardHandler = kbHandler;
            webBrowser.MenuHandler = new ContextMenuInterceptor();
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

        internal async void ToggleFullscreen()
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
            await settings.SaveAsync();
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
            SplashScreen.Instance.InvokeUI(() =>
            {
                SplashScreen.Instance.Close();
            });
            OverlayForm.Instance.Size = webBrowser.Size;
            var webViewBounds = new Rectangle(webBrowser.PointToScreen(Point.Empty), webBrowser.Size);
            OverlayForm.Instance.Location = webViewBounds.Location;
            Activate();
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
            if (kbHandler.hasHangingAltKey) //Alt-Tab fix for game
                SendKeys.Send("%{F16}");

            if (!OverlayForm.Instance.IsDisposed)
                OverlayForm.Instance.Visible = true;
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.None:
                    e.Cancel = true;
                    break;
                case CloseReason.UserClosing:
                    audioHandler.DestroySession();
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

        private async void GameForm_ResizeEnd(object sender, EventArgs e)
        {
            CaptureCursor();
            settings.SetWindowSize(Size);
            await settings.SaveAsync();
        }

        internal async void MouseLock(bool choice)
        {
            mouseLocked = choice;
            CaptureCursor();
            settings.SetMouseLock(mouseLocked);
            await settings.SaveAsync();

        }

        internal void AddonsLoadedPostLogic()
        {
            gameHasLoaded = true;
            ForceResizeGameWindows();
            loadingPanel.Visible = false;
            loadingTimer.Enabled = false;
        }

        internal void ChangeVolume(float value)
        {
            audioHandler.ChangeVolume(value);
        }

        internal async void VolumeChangePostLogic(float value)
        {
            audioHandler.ChangeVolume(value);
            settings.SetVolume(value);
            await settings.SaveAsync();
        }

        private void webView_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                InvokeUI(() =>
                {
                    loaderImage.Visible = true;
                    loadingPanel.Visible = true;
                    loadingText.Text = "Loading";
                    loadingText.Enabled = true;
                    loadingTimer.Enabled = true;
                    gameHasLoaded = false;
                });
            }
            else // has loaded
            {
                if (requestCallWhereLoadingFinished < requestCallCounter)
                {
                    requestCallWhereLoadingFinished = requestCallCounter;
                    var addonJS = System.IO.File.ReadAllText("js/addons.js");
                    webBrowser.ExecuteScriptAsync(addonJS);
                    ElementMessage.CallJSFunc(webBrowser, "init.function", $"\"{versionHandler.CurrentVersion}\", {settings.GetMouseLock().ToString().ToLower()}, {settings.GetVolume()}");
                    kbHandler.InitHotkeyNames((ChromiumWebBrowser)sender, settings);
                }
            }
        }

        private void webView_LoadError(object sender, LoadErrorEventArgs e)
        {
            InvokeUI(() =>
            {
                loaderImage.Visible = false;
                loadingText.Text = "ERROR";
                loadingText.Enabled = false;
                loadingText.Visible = true;
                loadingTimer.Enabled = false;
                gameHasLoaded = false;
            });
            OverlayForm.Instance.AddOverlayMessage("loadError", new Notification("Error: Website could not be loaded"));
        }
    }
}
