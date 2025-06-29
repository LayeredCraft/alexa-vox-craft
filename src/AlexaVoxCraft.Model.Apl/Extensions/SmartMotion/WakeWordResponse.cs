using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<WakeWordResponse>))]
public enum WakeWordResponse
{
    [EnumMember(Value="doNotMoveOnWakeWord")]
    DoNotMoveOnWakeWord,
    [EnumMember(Value="followOnWakeWord")]
    FollowOnWakeWord,
    [EnumMember(Value="turnToWakeWord")]
    TurnToWakeWord
}