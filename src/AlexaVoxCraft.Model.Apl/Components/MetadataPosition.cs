using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<MetadataPosition>))]
public enum MetadataPosition
{
    [EnumMember(Value="above_right")]
    AboveRight,
    [EnumMember(Value="above_left_right")]
    AboveLeftRight,
    [EnumMember(Value="below_left_right")]
    BelowLeftRight
}