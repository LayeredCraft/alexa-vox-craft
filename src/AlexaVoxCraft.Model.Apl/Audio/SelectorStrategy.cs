using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Audio;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<SelectorStrategy>))]
public enum SelectorStrategy
{
    [EnumMember(Value="normal")]
    Normal,
    [EnumMember(Value="randomItem")]
    RandomItem,
    [EnumMember(Value="randomData")]
    RandomData,
    [EnumMember(Value="randomItemRandomData")]
    RandomItemRandomData
}