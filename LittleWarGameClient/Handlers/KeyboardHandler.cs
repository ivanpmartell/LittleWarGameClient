using System.Reflection;
using CefSharp.WinForms;
using CefSharp;
using LittleWarGameClient.Helpers;

namespace LittleWarGameClient.Handlers
{
    internal class KeyboardHandler : IKeyboardHandler
    {
        private readonly Dictionary<Keys, MethodInfo?> hotKeys = new Dictionary<Keys, MethodInfo?>();
        internal bool hasHangingAltKey = false;

        internal KeyboardHandler(SettingsHandler settings)
        {
            InitHotkeys(settings);
        }

        private void InitHotkeys(SettingsHandler settings)
        {
            Type type = typeof(SettingsHandler);
            foreach (var methodInfo in type.GetMethods().Where(p => Attribute.IsDefined(p, typeof(Hotkey))))
            {
                object[] attribute = methodInfo.GetCustomAttributes(typeof(Hotkey), true);
                MethodInfo? funcToCall = null;
                if (attribute.Length > 0)
                {
                    Hotkey hotkey = (Hotkey)attribute[0];
                    if (hotkey.FuncToCall != null)
                    {
                        Type thisType = GetType();
                        funcToCall = thisType.GetMethod(hotkey.FuncToCall, BindingFlags.NonPublic | BindingFlags.Instance);
                    }
                }
                var key = methodInfo.Invoke(settings, null);
                if (key != null)
                    hotKeys.Add((Keys)key, funcToCall);
            }
        }

        internal void InitHotkeyNames(ChromiumWebBrowser sender, SettingsHandler settings)
        {
            Type type = typeof(SettingsHandler);
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
                    ElementMessage.CallJSFunc(sender, jsFuncToCall, $"\"{(Keys)key}\"");
            }
        }

        private void FullscreenHotkeyFunc(ChromiumWebBrowser sender)
        {
            GameForm.Instance.InvokeUI(() =>
            {
                GameForm.Instance.ToggleFullscreen();
            });
        }

        private void OptionsMenuHotkeyFunc(ChromiumWebBrowser sender)
        {
            ElementMessage.CallJSFunc(sender, "toggleMenu");
        }

        private void ChatHistoryHotkeyFunc(ChromiumWebBrowser sender)
        {
            ElementMessage.CallJSFunc(sender, "toggleChat");
        }

        private void FriendsHotkeyFunc(ChromiumWebBrowser sender)
        {
            ElementMessage.CallJSFunc(sender, "toggleFriends");
        }

        public bool OnPreKeyEvent(IWebBrowser webView, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (OverlayForm.Instance.IsActivated)
                return true;

            var key = (Keys)windowsKeyCode;
            if (type == KeyType.KeyUp)
            {
                if (key == Keys.Menu) // Alt key
                    hasHangingAltKey = false;
            }
            if (type == KeyType.RawKeyDown)
            {
#if DEBUG
                if (key == Keys.F12)
                {
                    webView.ShowDevTools();
                    return true;
                }
#endif
                if (key == Keys.Menu) // Alt key
                    hasHangingAltKey = true;

                if (hotKeys.ContainsKey(key))
                {
                    var funcToCall = hotKeys[key];
                    if (funcToCall != null && webView != null)
                        funcToCall.Invoke(this, new object[] { (ChromiumWebBrowser)webView });
                    return true;
                }
            }
            return false;
        }

        public bool OnKeyEvent(IWebBrowser webView, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            if (isSystemKey && type == KeyType.Char)
                return true;
            //Matches Alt+F16 key press related to Alt-Tab fix for game
            if (windowsKeyCode == 0x7F && modifiers == CefEventFlags.AltDown)
                hasHangingAltKey = false;
            return false;
        }
    }
}
