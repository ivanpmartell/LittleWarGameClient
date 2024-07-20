using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Reflection.Metadata;

namespace LittleWarGameClient
{
    public partial class Form1 : Form
    {
        readonly Settings settings;
        readonly Fullscreen fullScreen;
        readonly KeyboardHandler kbHandler;
        readonly VersionHandler vHandler;
        bool wasSmallWindow = false;
        bool gameHasLoaded = false;
        bool mouseLocked;
        public Form1()
        {
            InitializeComponent();
            ChangeEnvironment();
            settings = new Settings();
            this.Size = settings.GetWindowSize();
            fullScreen = new Fullscreen(this, settings);
            kbHandler = new KeyboardHandler(webView, fullScreen, settings);
            vHandler = new VersionHandler(settings);
            mouseLocked = settings.GetMouseLock();
        }

        private async void ChangeEnvironment()
        {
            var path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, Path.Join(path, "data"), new CoreWebView2EnvironmentOptions());
            await webView.EnsureCoreWebView2Async(env);
            webView.Source = new Uri("https://littlewargame.com/play", UriKind.Absolute);
        }

        private void webView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            var addOnJS = System.IO.File.ReadAllText("AddOns.js");
            webView.CoreWebView2.ExecuteScriptAsync(addOnJS);
            ElementMessage.CallJSFunc(webView, "init.function", $"{settings.GetMouseLock().ToString().ToLower()}, \"{vHandler.CurrentVersion}\"");
            kbHandler.InitHotkeyNames(settings);
            gameHasLoaded = true;
            ResizeGameWindows();
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
                    default:
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
            settings.Save();
        }
    }
}
