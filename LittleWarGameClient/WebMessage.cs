using CefSharp;
using CefSharp.WinForms;
using System.Runtime.Serialization;
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
