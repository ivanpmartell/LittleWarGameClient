using Microsoft.Web.WebView2.WinForms;
using System;
using System.Reflection.Metadata;
using System.Reflection;

namespace LittleWarGameClient
{
    internal class KeyboardHandler
    {
        private readonly WebView2 webView;
        private readonly Fullscreen fullScreen;
        private readonly Dictionary<Keys, MethodInfo?> hotKeys = new Dictionary<Keys, MethodInfo?>();

        internal KeyboardHandler(WebView2 wv, Fullscreen fs, Settings settings)
        {
            InitHotkeys(settings);
            fullScreen = fs;
            webView = wv;
            webView.KeyDown += TargetWebView_KeyDown;
            webView.KeyUp += TargetWebView_KeyUp;
        }

        private void InitHotkeys(Settings settings)
        {
            Type type = typeof(Settings);
            foreach (var methodInfo in type.GetMethods().Where(p => Attribute.IsDefined(p, typeof(Hotkey))))
            {
                object[] attribute = methodInfo.GetCustomAttributes(typeof(Hotkey), true);
                MethodInfo? funcToCall = null;
                if (attribute.Length > 0)
                {
                    Hotkey hotkey = (Hotkey)attribute[0];
                    if (hotkey.FuncToCall != null)
                    {
                        Type thisType = this.GetType();
                        funcToCall = thisType.GetMethod(hotkey.FuncToCall, BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                }
                var key = methodInfo.Invoke(settings, null);
                if (key != null)
                    hotKeys.Add((Keys)key, funcToCall);
            }
        }

        internal void InitHotkeyNames(Settings settings)
        {
            Type type = typeof(Settings);
            foreach (var methodInfo in type.GetMethods().Where(p => Attribute.IsDefined(p, typeof(Hotkey))))
            {
                object[] attribute = methodInfo.GetCustomAttributes(typeof(Hotkey), true);
                string? jsFuncToCall = null;
                if (attribute.Length > 0)
                {
                    Hotkey hotkey = (Hotkey)attribute[0];
                    jsFuncToCall = hotkey.JSFuncToCall;
                }
                var key = methodInfo.Invoke(settings, null);
                if (jsFuncToCall != null && key != null)
                    ElementMessage.CallJSFunc(webView, jsFuncToCall, $"\"{(Keys)key}\"");
            }
        }

        private void TargetWebView_KeyUp(object? sender, KeyEventArgs e)
        { 
            if (hotKeys.ContainsKey(e.KeyData))
            {
                var funcToCall = hotKeys[e.KeyData];
                if (funcToCall != null && sender != null)
                    funcToCall.Invoke(this, new object[] { (WebView2)sender });
                e.Handled = true;
            }
        }

        private void FullscreenHotkeyFunc(WebView2 sender)
        {
            fullScreen.Toggle();
        }

        private void OptionsMenuHotkeyFunc(WebView2 sender)
        {
            ElementMessage.CallJSFunc(sender, "toggleMenu");
        }

        private void ChatHistoryHotkeyFunc(WebView2 sender)
        {
            ElementMessage.CallJSFunc((WebView2)sender, "toggleChat");
        }

        private void FriendsHotkeyFunc(WebView2 sender)
        {
            ElementMessage.CallJSFunc((WebView2)sender, "toggleFriends");
        }

        private void TargetWebView_KeyDown(object? sender, KeyEventArgs e)
        {
#if DEBUG
            if (e.KeyData == Keys.F12)
            {
                webView.CoreWebView2.OpenDevToolsWindow();
                e.Handled = true;
                return;
            }
#endif
            if (hotKeys.ContainsKey(e.KeyData))
                e.Handled = true;
        }
    }
}
