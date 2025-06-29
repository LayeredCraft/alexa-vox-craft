using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<Scale>))]
public enum Scale
{
    [EnumMember(Value = "none")]
    None,
    [EnumMember(Value = "fill")]
    Fill,
    [EnumMember(Value = "best-fill")]
    BestFill,
    [EnumMember(Value = "best-fit")]
    BestFit,
    [EnumMember(Value = "best-fit-down")]
    BestFitDown
}