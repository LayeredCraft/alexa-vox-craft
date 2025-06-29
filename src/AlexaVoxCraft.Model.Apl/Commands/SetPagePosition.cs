using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Commands;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<SetPagePosition>))]
public enum SetPagePosition
{
    [EnumMember(Value="absolute")]
    Absolute,
    [EnumMember(Value="relative")]
    Relative
}