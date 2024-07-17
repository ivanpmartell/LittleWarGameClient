using System.Runtime.Serialization;

namespace LittleWarGameClient
{
    public class ButtonPress
    {
        public string? ElementId { get; set; }
        public string? ElementType { get; set; }
        public ButtonType? Value { get; set; }
    }

    public enum ButtonType
    {
        [EnumMember(Value = "FullScreen")]
        FullScreen,
        [EnumMember(Value = "Exit")]
        Exit
    }
}
