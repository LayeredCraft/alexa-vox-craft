using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<MediaTagState>))]
public enum MediaTagState
{
    [EnumMember(Value="idle")]
    Idle,
    [EnumMember(Value="paused")]
    Paused,
    [EnumMember(Value="playing")]
    Playing
}