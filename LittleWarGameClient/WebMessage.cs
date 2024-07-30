using CefSharp;
using CefSharp.WinForms;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LittleWarGameClient
{
    internal class ElementMessage
    {
        public string? Id { get; set; }
        public string? Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ButtonType? Type { get; set; }

        internal static void CallJSFunc(ChromiumWebBrowser receiver, string func, string args = "")
        {
            var script = $"addons.{func}({args})";
            receiver.ExecuteScriptAsync(script);
        }

        internal static void JSMessageReceived(object? sender, JavascriptMessageReceivedEventArgs e)
        {
            ElementMessage? msg = JsonSerializer.Deserialize<ElementMessage>((string)e.Message);
            if (msg != null)
            {
                switch (msg.Type)
                {
                    case ButtonType.FullScreen:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.ToggleFullscreen();
                        });
                        break;
                    case ButtonType.Exit:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.Close();
                        });
                        break;
                    case ButtonType.MouseLock:
                        if (msg.Value != null)
                            GameForm.Instance.InvokeUI(() =>
                            {
                                GameForm.Instance.MouseLock(bool.Parse(msg.Value));
                            });
                        break;
                    case ButtonType.InitComplete:
                        GameForm.Instance.InvokeUI(() =>
                        {
                            GameForm.Instance.AddonsLoadedPostLogic();
                        });
                        break;
                    case ButtonType.VolumeChanging:
                        if (msg.Value != null)
                        {
                            GameForm.Instance.ChangeVolume(float.Parse(msg.Value));
                        }
                        break;
                    case ButtonType.VolumeChanged:
                        if (msg.Value != null)
                        {
                            GameForm.Instance.VolumeChangePostLogic(float.Parse(msg.Value));
                        }
                        break;
                }
            }
        }
    }

    internal enum ButtonType
    {
        [EnumMember(Value = "FullScreen")]
        FullScreen,
        [EnumMember(Value = "Exit")]
        Exit,
        [EnumMember(Value = "MouseLock")]
        MouseLock,
        [EnumMember(Value = "InitComplete")]
        InitComplete,
        [EnumMember(Value = "VolumeChanging")]
        VolumeChanging,
        [EnumMember(Value = "VolumeChanged")]
        VolumeChanged
    }
}
