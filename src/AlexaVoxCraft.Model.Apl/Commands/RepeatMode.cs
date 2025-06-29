using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Commands;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<RepeatMode>))]
public enum RepeatMode
{
    [EnumMember(Value="restart")]
    Restart,
    [EnumMember(Value="reverse")]
    Reverse
}