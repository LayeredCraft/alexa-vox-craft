using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Apl.Gestures;

public enum SwipeAction
{
    [EnumMember(Value="reveal")]
    Reveal,
    [EnumMember(Value="slide")]
    Slide,
    [EnumMember(Value="cover")]
    Cover
}