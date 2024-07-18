using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace LittleWarGameClient
{
    public class ElementMessage
    {
        public string? Id { get; set; }
        public string? Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ButtonType? Type { get; set; }
    }

    public enum ButtonType
    {
        [EnumMember(Value = "FullScreen")]
        FullScreen,
        [EnumMember(Value = "Exit")]
        Exit,
        [EnumMember(Value = "MouseLock")]
        MouseLock
    }
}
