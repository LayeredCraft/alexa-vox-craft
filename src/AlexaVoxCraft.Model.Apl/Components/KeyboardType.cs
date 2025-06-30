using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<KeyboardType>))]
public enum KeyboardType
{
    [EnumMember(Value="decimalPad")]
    DecimalPad,
    [EnumMember(Value="emailAddress")]
    EmailAddress,
    [EnumMember(Value="normal")]
    Normal,
    [EnumMember(Value="numberPad")]
    NumberPad,
    [EnumMember(Value="phonePad")]
    PhonePad,
    [EnumMember(Value="url")]
    Url
}