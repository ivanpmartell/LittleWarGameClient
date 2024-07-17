using Microsoft.Web.WebView2;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LittleWarGameClient
{
    public partial class Form1 : Form
    {
        Fullscreen fullScreen;
        KeyboardHandler kbHandler;
        public Form1()
        {
            InitializeComponent();
            fullScreen = new Fullscreen(this);
            kbHandler = new KeyboardHandler(webView, fullScreen);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            webView.Size = this.ClientSize - new System.Drawing.Size(webView.Location);
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            string text = System.IO.File.ReadAllText("AddOns.js");
            webView.CoreWebView2.ExecuteScriptAsync(text);
            webView.CoreWebView2.ExecuteScriptAsync("addons.init.function()");
        }

        private void webView_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter<ButtonType>() }
            };
            ButtonPress? button = JsonSerializer.Deserialize<ButtonPress>(e.TryGetWebMessageAsString(), options);
            if (button != null)
            {
                switch (button.Value)
                {
                    case ButtonType.FullScreen:
                        fullScreen.Toggle();
                        break;
                    case ButtonType.Exit:
                        Application.Exit();
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
    }
}
