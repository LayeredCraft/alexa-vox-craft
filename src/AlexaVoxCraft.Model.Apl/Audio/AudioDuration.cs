using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Audio;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<AudioDuration>))]
public enum AudioDuration
{
    [EnumMember(Value="auto")]
    Auto,
    [EnumMember(Value="trimToParent")]
    TrimToParent
}