using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Commands;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<SetStateStates>))]
public enum SetStateStates
{
    [EnumMember(Value = "checked")]
    Checked,
    [EnumMember(Value = "disabled")]
    Disabled,
    [EnumMember(Value = "focused")]
    Focused,
    [EnumMember(Value = "karaoke")]
    Karaoke,
    [EnumMember(Value = "karaokeTarget")]
    KaraokeTarget,
    [EnumMember(Value = "pressed")]
    Pressed
}