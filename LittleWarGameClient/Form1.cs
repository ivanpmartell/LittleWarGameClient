using Microsoft.Web.WebView2;
using Microsoft.Web.WebView2.Core;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.IO;

namespace LittleWarGameClient
{
    public partial class Form1 : Form
    {
        readonly Settings settings;
        readonly Fullscreen fullScreen;
        readonly KeyboardHandler kbHandler;
        bool mouseLocked;
        public Form1()
        {
            InitializeComponent();
            ChangeEnvironment();
            settings = new Settings();
            this.Size = settings.GetWindowSize();
            fullScreen = new Fullscreen(this, settings);
            kbHandler = new KeyboardHandler(webView, fullScreen);
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
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            string text = System.IO.File.ReadAllText("AddOns.js");
            webView.CoreWebView2.ExecuteScriptAsync(text);
            webView.CoreWebView2.ExecuteScriptAsync($"addons.init.function({settings.GetMouseLock().ToString().ToLower()})");
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
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            CaptureCursor();
            settings.SetWindowSize(this.Width, this.Height);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CaptureCursor();
        }
    }
}
