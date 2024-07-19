using Microsoft.Web.WebView2.WinForms;
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

        internal static async void CallJSFunc(WebView2 receiver, string func, string args = "")
        {
            var script = $"addons.{func}({args})";
            await receiver.CoreWebView2.ExecuteScriptAsync(script);
        }
    }

    internal enum ButtonType
    {
        [EnumMember(Value = "FullScreen")]
        FullScreen,
        [EnumMember(Value = "Exit")]
        Exit,
        [EnumMember(Value = "MouseLock")]
        MouseLock
    }
}
