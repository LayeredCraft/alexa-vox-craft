using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<AlexaImageAlignment>))]
public enum AlexaImageAlignment
{
    [EnumMember(Value="bottom")]
    Bottom,
    [EnumMember(Value = "bottom-left")]
    BottomLeft,
    [EnumMember(Value = "bottom-right")]
    BottomRight,
    [EnumMember(Value = "center")]
    Center,
    [EnumMember(Value = "left")]
    Left,
    [EnumMember(Value = "right")]
    Right,
    [EnumMember(Value = "top")]
    Top,
    [EnumMember(Value = "top-left")]
    TopLeft,
    [EnumMember(Value = "top-right")]
    TopRight,
}