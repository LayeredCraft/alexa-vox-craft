using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<TextOverflow>))]
public enum TextOverflow
{
    [EnumMember(Value= "marquee")]
    Marquee,
    [EnumMember(Value="wrap")]
    Wrap
}