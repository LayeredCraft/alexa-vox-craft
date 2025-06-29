using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<SliderType>))]
public enum SliderType
{
    [EnumMember(Value="default")]
    Default,
    [EnumMember(Value="icon")]
    Icon,
    [EnumMember(Value="text")]
    Text
}