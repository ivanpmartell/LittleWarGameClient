using Microsoft.Web.WebView2;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace LittleWarGameClient
{
    public partial class Form1 : Form
    {
        Fullscreen fullScreen;
        KeyboardHandler kbHandler;
        bool mouseLocked;
        public Form1()
        {
            InitializeComponent();
            fullScreen = new Fullscreen(this);
            kbHandler = new KeyboardHandler(webView, fullScreen);
            mouseLocked = false;
        }

        private void CaptureCursor()
        {
            if (mouseLocked)
            {
                this.Capture = true;
                var webViewBounds = new Rectangle(webView.PointToScreen(Point.Empty), webView.Size);
                Cursor.Clip = webViewBounds;
            }
            else
            {
                this.Capture = false;
                Cursor.Clip = Rectangle.Empty;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            webView.Size = this.ClientSize - new System.Drawing.Size(webView.Location);
            CaptureCursor();
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            string text = System.IO.File.ReadAllText("AddOns.js");
            webView.CoreWebView2.ExecuteScriptAsync(text);
            webView.CoreWebView2.ExecuteScriptAsync("addons.init.function()");
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
                        CaptureCursor();
                        break;
                    default:
                        break;
                }
            }
        }

        private void webView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            CaptureCursor();
        }
    }
}
