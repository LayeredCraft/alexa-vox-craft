using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<TimeTextDirection>))]
public enum TimeTextDirection
{
    [EnumMember(Value="none")]
    None,
    [EnumMember(Value = "up")]
    Up,
    [EnumMember(Value = "down")]
    Down,
}