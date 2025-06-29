using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public enum SwipeDirection
{
    [EnumMember(Value="left")]
    Left,
    [EnumMember(Value="right")]
    Right,
    [EnumMember(Value="up")]
    Up,
    [EnumMember(Value="down")]
    Down,
    [EnumMember(Value="forward")]
    Forward,
    [EnumMember(Value="backward")]
    Backward
}