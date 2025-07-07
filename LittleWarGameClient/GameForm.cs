using CefSharp;
using CefSharp.WinForms;
using LittleWarGameClient.Handlers;
using LittleWarGameClient.Helpers;
using LittleWarGameClient.Interceptors;
using System.Text;
using System.Text.Json;

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
        private readonly List<string> enabledPluginScripts = new();
        private readonly SettingsHandler settings = new();
        private readonly KeyboardHandler kbHandler;
        private readonly VersionHandler versionHandler;
        private readonly AudioHandler audioHandler;
        private readonly PluginHandler pluginHandler;
        private FormWindowState PreviousWindowState;

        internal int requestCallCounter = 0;
        private int requestCallWhereLoadingFinished = -1;
        private bool wasSmallWindow = false;
        private bool gameHasLoaded = false;
        private bool mouseLocked;
        

        internal bool isOverlayActivated = false;

        internal GameForm()
        {
            if (InstanceName == null)
                throw new MissingFieldException(nameof(InstanceName));
            string exeDirectory = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)!;
            PreInitWeb(exeDirectory);
            InitializeComponent();
            Text = $"Littlewargame({InstanceName})";
            InitLoadingScreen();
            audioHandler = new AudioHandler(Text);
            kbHandler = new KeyboardHandler(settings);
            versionHandler = new VersionHandler(settings);
            pluginHandler = new PluginHandler(exeDirectory, settings);
            InitScreen();
            InitWebView();
        }

        private void InitLoadingScreen()
        {
            loadingText.Font = FontHandler.gameFont(48F);
            loadingPanel.SetDoubleBuffered();
            loadingPanel.BringToFront();
        }

        private void PreInitWeb(string? exeDirectory)
        {
            var cefSettings = new CefSettings();
            cefSettings.CefCommandLineArgs.Add("no-proxy-server", "1");
            cefSettings.CefCommandLineArgs.Add("disable-plugins-discovery", "1");
            cefSettings.CefCommandLineArgs.Add("disable-extensions", "1");
            cefSettings.RootCachePath = Path.Join(exeDirectory, "data", InstanceName);
            Cef.Initialize(cefSettings);
        }

        private void InitPlugins()
        {
            var enabledPluginsCount = 0;
            if (!settings.GetDisableAllPlugins())
            {
                foreach (var (pluginId, plugin) in pluginHandler.GetInstalledPlugins())
                {
                    if (plugin.Enabled)
                    {
                        enabledPluginsCount++;
                        LoadPlugin(pluginId);
                    }
                }
            }
            if (enabledPluginsCount == 0)
            {
                mainImage.Image = Properties.Resources.soldier;
                mainImage.Location = new Point(582, 70);
                mainImage.Size = new Size(100, 100);
            }
        }

        private void InitPluginsGameScript()
        {
            if (!settings.GetDisableAllPlugins())
            {
                pluginHandler.SynchronizeWithLocalPlugins();
                enabledPluginScripts.Clear();
                foreach (var (pluginId, plugin) in pluginHandler.GetInstalledPlugins())
                {
                    if (plugin.Enabled)
                    {
                        if (plugin.GameScript != null)
                        {
                            string gameScriptPath = Path.Combine(plugin.AbsolutePluginPath!, plugin.GameScript);
                            if (File.Exists(gameScriptPath))
                                webBrowser.RequestHandler = new RequestInterceptor(gameScriptPath);
                        }
                        else
                            webBrowser.RequestHandler = new DefaultRequestInterceptor();
                    }
                }
            }
        }

        private void LoadPlugin(string pluginId)
        {
            Plugin plugin = pluginHandler.GetInstalledPlugins()[pluginId];
            if (plugin.Image != null)
            {
                var imagePath = Path.Combine(plugin.AbsolutePluginPath!, plugin.Image);
                mainImage.LoadAsync(imagePath);
            }
            if (plugin.ImageLocation != null)
            {
                mainImage.Location = (Point)plugin.ImageLocation;
            }
            if (plugin.ImageSize != null)
            {
                mainImage.Size = (Size)plugin.ImageSize;
            }
            foreach (string script in plugin.Scripts)
            {
                var scriptPath = Path.Combine(plugin.AbsolutePluginPath!, script);
                enabledPluginScripts.Add(scriptPath);
            }
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

        private void InitWebView()
        {
            webBrowser.JavascriptMessageReceived += ElementMessage.JSMessageReceived;
            webBrowser.KeyboardHandler = kbHandler;
            webBrowser.MenuHandler = new ContextMenuInterceptor();
            webBrowser.DownloadHandler = new DownloadInterceptor();
            webBrowser.RequestHandler = new DefaultRequestInterceptor();
            webBrowser.LoadUrl(baseUrl);
        }

        internal async void OverlayChanged(int overlayChoice)
        {
            if (Enum.IsDefined(typeof(OverlayType), overlayChoice))
            {
                OverlayType overlayType = (OverlayType)overlayChoice;
                if (overlayType == settings.GetOverlayType())
                    return;
                await OverlayHelper.Instance.StopAsync();
                switch (settings.GetOverlayType())
                {
                    case OverlayType.Direct2D:
                        D2DOverlay.Instance.Close();
                        break;
                    case OverlayType.OpenGL:
                        OpenGLOverlay.Instance.Close();
                        break;
                }
                settings.SetOverlayType(overlayType);
                await settings.SaveAsync();
                var prevWindowState = WindowState;
                WindowState = FormWindowState.Minimized;
                WindowState = prevWindowState;
            }
        }

        internal void EnablePlugin(string pluginId)
        {
            Plugin plugin = pluginHandler.GetInstalledPlugins()[pluginId];
            bool requiresRefresh = false;
            if (plugin.ModifiesGameScript())
            {
                var conflictingPlugins = pluginHandler.GetEnabledPluginIdsThatModifyGameScript();
                if (!ContinueWithConflictingPlugins(plugin, conflictingPlugins, "game script"))
                    return;
                DisablePlugins(conflictingPlugins);
                requiresRefresh = true;
            }

            if (plugin.ModifiesLoadingImage())
            {
                var conflictingPlugins = pluginHandler.GetEnabledPluginIdsThatModifyLoadingImage();
                if (!ContinueWithConflictingPlugins(plugin, conflictingPlugins, "loading image"))
                    return;
                DisablePlugins(conflictingPlugins);
                requiresRefresh = true;
            }

            pluginHandler.EnableAPlugin(pluginId);
            if (requiresRefresh)
                RefreshInstalledPluginsTabContents();
            if (DialogResult.OK == MessageBox.Show("Enabling a plugin requires reloading the game. Reload now?", "Warning", MessageBoxButtons.OKCancel))
                ReloadGame();
        }

        internal void DisablePlugin(string pluginId)
        {
            if (!pluginHandler.GetInstalledPlugins()[pluginId].Enabled)
                return;
            pluginHandler.DisableAPlugin(pluginId);
            if (DialogResult.OK == MessageBox.Show("Disabling a plugin requires reloading the game. Reload now?", "Warning", MessageBoxButtons.OKCancel))
                ReloadGame();
        }

        private void RefreshInstalledPluginsTabContents()
        {
            ElementMessage.CallJSFunc(webBrowser, "clearInstalledPluginsTabContents");
            SendInstalledPluginsAndLatestVersions();
        }

        private void RefreshAvailablePluginsTabContents()
        {
            ElementMessage.CallJSFunc(webBrowser, "clearAvailablePluginsTabContents");
            SendAvailablePlugins();
        }

        internal void InstallPlugin(string pluginId)
        {
            if (pluginHandler.InstallOrUpdateAPlugin(pluginId))
            {
                RefreshAvailablePluginsTabContents();
                MessageBox.Show("Plugin was successfully installed", "Success", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("There was an error installing the plugin", "Error", MessageBoxButtons.OK);
            }
        }

        internal void UninstallPlugin(string pluginId)
        {
            pluginHandler.UninstallAPlugin(pluginId);
            RefreshInstalledPluginsTabContents();
            MessageBox.Show("Plugin was successfully uninstalled", "Success", MessageBoxButtons.OK);
        }

        internal void UpdatePlugin(string pluginId)
        {
            if (pluginHandler.InstallOrUpdateAPlugin(pluginId))
            {
                RefreshInstalledPluginsTabContents();
                MessageBox.Show("Plugin was successfully updated", "Success", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("There was an error updating the plugin", "Error", MessageBoxButtons.OK);
            }
        }

        private bool ContinueWithConflictingPlugins(Plugin plugin, List<string> conflictingPlugins, string conflictReason)
        {
            if (conflictingPlugins.Count > 0)
            {
                string conflictingPluginNames = "";
                foreach (string conflictingPlugin in conflictingPlugins)
                    conflictingPluginNames += "\n\t" + pluginHandler.GetInstalledPlugins()[conflictingPlugin].Name;
                if (DialogResult.Cancel == MessageBox.Show($"The following plugins also modify the {conflictReason} and will conflict with this plugin:{conflictingPluginNames}\nIf you continue, they will be disabled.\nContinue?", "Warning", MessageBoxButtons.OKCancel))
                {
                    ElementMessage.CallJSFunc(webBrowser, "cancelledEnablePlugin", $"\"{plugin.Folder}\"");
                    return false;
                }
            }
            return true;
        }

        private void DisablePlugins(List<string> conflictingPlugins)
        {
            foreach (string conflictingPlugin in conflictingPlugins)
                pluginHandler.DisableAPlugin(conflictingPlugin);
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

        internal void ReloadGame()
        {
            webBrowser.Reload(true);
        }

        internal void DisableAllPlugins(bool option)
        {
            settings.SetDisableAllPlugins(option);
            if (DialogResult.OK == MessageBox.Show("Disabling the client's plugin functionality requires reloading the game. Reload now?", "Warning", MessageBoxButtons.OKCancel))
                ReloadGame();
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
            switch (settings.GetOverlayType())
            {
                case OverlayType.Direct2D:
                    D2DOverlay.Instance.Location = webViewBounds.Location;
                    break;
                case OverlayType.OpenGL:
                    OpenGLOverlay.Instance.Location = webViewBounds.Location;
                    break;
            }
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            SplashScreen.Instance.InvokeUI(() =>
            {
                SplashScreen.Instance.Close();
                SplashScreen.Instance.Dispose();
            });
            var webViewBounds = new Rectangle(webBrowser.PointToScreen(Point.Empty), webBrowser.Size);
            switch (settings.GetOverlayType())
            {
                case OverlayType.Direct2D:
                    D2DOverlay.Instance.Size = webBrowser.Size;
                    D2DOverlay.Instance.Location = webViewBounds.Location;
                    break;
                case OverlayType.OpenGL:
                    OpenGLOverlay.Instance.Size = webBrowser.Size;
                    OpenGLOverlay.Instance.Location = webViewBounds.Location;
                    break;
            }
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
            if (!GameForm.Instance.isOverlayActivated)
            {
                switch (settings.GetOverlayType())
                {
                    case OverlayType.Direct2D:
                            D2DOverlay.Instance.Visible = false;
                        break;
                    case OverlayType.OpenGL:
                            OpenGLOverlay.Instance.Visible = false;
                        break;
                }
            }
        }

        private void GameForm_Activated(object sender, EventArgs e)
        {
            CaptureCursor();
            ResizeGameWindows();
            if (kbHandler.hasHangingAltKey) //Alt-Tab fix for game
                SendKeys.Send("%{F16}");

            switch (settings.GetOverlayType())
            {
                case OverlayType.Direct2D:
                    if (!D2DOverlay.Instance.IsDisposed)
                        D2DOverlay.Instance.Visible = true;
                    break;
                case OverlayType.OpenGL:
                    if (!OpenGLOverlay.Instance.IsDisposed)
                        OpenGLOverlay.Instance.Visible = true;
                    break;
            }
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
                    webBrowser.CloseDevTools();
                    webBrowser.Dispose();
                    Application.Exit();
                    break;
            }
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            switch (settings.GetOverlayType())
            {
                case OverlayType.Direct2D:
                    D2DOverlay.Instance.Size = webBrowser.Size;
                    break;
                case OverlayType.OpenGL:
                    //OpenGL specific fix where fullscreen does not allow for transparency
                    OpenGLOverlay.Instance.Size = new Size(webBrowser.Size.Width, webBrowser.Size.Height - 1);
                    break;
            }

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
            if (!settings.GetDisableAllPlugins())
            {
                foreach (var script in enabledPluginScripts)
                {
                    var scriptJS = System.IO.File.ReadAllText(script);
                    webBrowser.ExecuteScriptAsync(scriptJS);
                }
            }
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
            if (!e.IsLoading)
            {
                if (requestCallWhereLoadingFinished < requestCallCounter)
                {
                    requestCallWhereLoadingFinished = requestCallCounter;
                    var addonJS = Encoding.Default.GetString(Properties.Resources.addons);
                    webBrowser.ExecuteScriptAsync(addonJS);
                    var overlayOptions = SettingsHelper.EnumToCommaSeparatedString<OverlayType>();
                    ElementMessage.CallJSFunc(webBrowser, "init.function", $"\"{versionHandler.CurrentVersion}\", {settings.GetMouseLock().ToString().ToLower()}, {settings.GetVolume()}, {settings.GetDisableAllPlugins().ToString().ToLower()}, {((int)settings.GetOverlayType())}, \"{overlayOptions}\"");
                    kbHandler.InitHotkeyNames((ChromiumWebBrowser)sender, settings);
                }
            }
            else
            {
                InitPluginsGameScript();
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
            OverlayHelper.Instance.AddOverlayMessage("loadError", new Notification("Error: Website could not be loaded"));
        }

        private void webBrowser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            InvokeUI(() =>
            {
                InitPlugins();
                loaderImage.Visible = true;
                loadingPanel.Visible = true;
                loadingText.Text = "Loading";
                loadingText.Enabled = true;
                loadingTimer.Enabled = true;
                gameHasLoaded = false;
                if (settings.GetDebugMode())
                    webBrowser.ShowDevTools();
            });
        }

        internal void SendAvailablePlugins()
        {
            var availablePlugins = pluginHandler.GetAvailablePluginsOnline();
            foreach (var (pluginId, plugin) in availablePlugins)
            {
                string json = JsonSerializer.Serialize(plugin);
                bool alreadyInstalled = pluginHandler.GetInstalledPlugins().ContainsKey(pluginId);
                ElementMessage.CallJSFunc(webBrowser, "receiveAvailablePlugin", $"{alreadyInstalled.ToString().ToLower()},\'{json}\'");
            }
        }

        internal void SendInstalledPluginsAndLatestVersions()
        {
            var latestVersions = pluginHandler.GetLatestVersionsOfAvailablePlugins();
            foreach (var (pluginId, plugin) in pluginHandler.GetInstalledPlugins())
            {
                string json = JsonSerializer.Serialize(plugin);
                string latestVersion = "null";
                if (latestVersions.ContainsKey(pluginId))
                   latestVersion = latestVersions[pluginId].ToString();
                ElementMessage.CallJSFunc(webBrowser, "receiveInstalledPlugin", $"\'{latestVersion}\',\'{json}\'");
            }
        }
    }

    internal readonly record struct Notification
    {
        internal string Message { get; }
        internal DateTime PostedTime { get; }

        internal Notification(string msg)
        {
            Message = msg;
            PostedTime = DateTime.Now;
        }
    }
}
