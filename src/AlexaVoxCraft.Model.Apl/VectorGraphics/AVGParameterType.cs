using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<AVGParameterType>))]
public enum AVGParameterType
{
    [EnumMember(Value="any")]
    Any,
    [EnumMember(Value = "string")]
    String,
    [EnumMember(Value = "number")]
    Number,
    [EnumMember(Value = "color")]
    Color,
}