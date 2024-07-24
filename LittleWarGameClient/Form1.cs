using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
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

namespace LittleWarGameClient
{
    internal partial class Form1 : Form
    {
        private const string baseUrl = @"https://littlewargame.com/play";
        private readonly Settings settings;
        private readonly Fullscreen fullScreen;
        private readonly KeyboardHandler kbHandler;
        private readonly VersionHandler vHandler;
        private readonly AudioManager audioMngr;

        private bool wasSmallWindow = false;
        private bool gameHasLoaded = false;
        private bool mouseLocked;

        public Form1()
        {
            InitializeComponent();
            InitWebView();
            audioMngr = new AudioManager(this.Text);
            settings = new Settings();
            this.Size = settings.GetWindowSize();
            fullScreen = new Fullscreen(this, settings);
            kbHandler = new KeyboardHandler(webView, fullScreen, settings);
            vHandler = new VersionHandler(settings);
            mouseLocked = settings.GetMouseLock();
        }

        private async void InitWebView()
        {
            loadingPanel.SetDoubleBuffered();
            var path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, Path.Join(path, "data"), new CoreWebView2EnvironmentOptions());
            await webView.EnsureCoreWebView2Async(env);
            webView.Source = new Uri(baseUrl, UriKind.Absolute);
            webView.CoreWebView2.Profile.DefaultDownloadFolderPath = Path.Join(path, "downloads");
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            var addonJS = System.IO.File.ReadAllText("addons.js");
            webView.CoreWebView2.ExecuteScriptAsync(addonJS);
            ElementMessage.CallJSFunc(webView, "init.function", $"\"{vHandler.CurrentVersion}\", {settings.GetMouseLock().ToString().ToLower()}, {settings.GetVolume()}");
            kbHandler.InitHotkeyNames(settings);
        }

        private void webView_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            ElementMessage? msg = JsonSerializer.Deserialize<ElementMessage>(e.TryGetWebMessageAsString());
            if (msg != null)
            {
                switch (msg.Type)
                {
                    case ButtonType.FullScreen:
                        fullScreen.Toggle();
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
                        CaptureCursor();
                        break;
                    case ButtonType.InitComplete:
                        gameHasLoaded = true;
                        ResizeGameWindows();
                        loadingPanel.Visible = false;
                        loadingTimer.Enabled = false;
                        loadingText.Text = "Reconnecting";
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
            settings.SetWindowSize(this.Size);
            settings.SaveAsync();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CaptureCursor();
            ResizeGameWindows();
        }

        private void ResizeGameWindows()
        {
            if (gameHasLoaded)
            {
                if (this.Height <= 800 && !wasSmallWindow)
                {
                    wasSmallWindow = true;
                    ElementMessage.CallJSFunc(webView, "setSmallWindowSizes");
                }
                else if (this.Height > 800 && wasSmallWindow)
                {
                    wasSmallWindow = false;
                    ElementMessage.CallJSFunc(webView, "setNormalWindowSizes");
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            audioMngr.DestroySession();
        }

        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            if (loadingText.Visible)
                loadingText.Visible = false;
            else
                loadingText.Visible = true;
        }

        private void webView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Script);
            webView.CoreWebView2.WebResourceRequested +=
                delegate (object? sender, CoreWebView2WebResourceRequestedEventArgs args)
                {
                    if (args.Request.Uri == $"{baseUrl}/js/lwg-5.0.0.js")
                    {
                        try
                        {
                            FileStream fs = File.Open("override/lwg-5.0.0.js", FileMode.Open);
                            CoreWebView2WebResourceResponse response = webView.CoreWebView2.Environment.CreateWebResourceResponse(fs, 200, "OK", "Content-Type: text/javascript");
                            args.Response = response;
                        }
                        catch { }
                    }
                };
            loadingPanel.Visible = true;
            loadingTimer.Enabled = true;
            gameHasLoaded = false;
        }
    }
}
