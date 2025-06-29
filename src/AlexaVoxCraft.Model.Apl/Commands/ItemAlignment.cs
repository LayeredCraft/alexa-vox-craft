using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Commands;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ItemAlignment>))]
public enum ItemAlignment
{
    [EnumMember(Value="first")]
    First,
    [EnumMember(Value = "center")]
    Center,
    [EnumMember(Value = "last")]
    Last,
    [EnumMember(Value = "visible")]
    Visible
}