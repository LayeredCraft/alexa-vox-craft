using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<APLGradientType>))]
public enum APLGradientType
{
    [EnumMember(Value="linear")]
    Linear,
    [EnumMember(Value = "radial")]
    Radial
}