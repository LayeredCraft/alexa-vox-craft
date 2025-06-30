using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<DrawOrder>))]
public enum DrawOrder
{
    [EnumMember(Value="nextAbove")]
    NextAbove,
    [EnumMember(Value = "nextBelow")]
    NextBelow,
    [EnumMember(Value = "higherAbove")]
    HigherAbove,
    [EnumMember(Value = "higherBelow")]
    HigherBelow
}