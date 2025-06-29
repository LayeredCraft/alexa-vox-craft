using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<APLTProfile>))]
public enum APLTProfile
{
    [EnumMember(Value="FOUR_CHARACTER_CLOCK")]
    FourCharacterClock
}