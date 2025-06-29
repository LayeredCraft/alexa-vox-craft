using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<AVGScaleType>))]
public enum AVGScaleType
{
    [EnumMember(Value="none")]
    None,
    [EnumMember(Value = "grow")]
    Grow,
    [EnumMember(Value = "shrink")]
    Shrink,
    [EnumMember(Value = "stretch")]
    Stretch
}