using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Commands;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ControlMediaCommand>))]
public enum ControlMediaCommand
{
    [EnumMember(Value="play")]
    Play,
    [EnumMember(Value = "pause")]
    Pause,
    [EnumMember(Value = "next")]
    Next,
    [EnumMember(Value = "previous")]
    Previous,
    [EnumMember(Value = "rewind")]
    Rewind,
    [EnumMember(Value = "seek")]
    Seek,
    [EnumMember(Value = "setTrack")]
    SetTrack,
    [EnumMember(Value = "seekTo")]
    SeekTo
}