using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ViewportShape>))]
public enum ViewportShape
{
    [EnumMember(Value="SQUARE")]
    Square,
    [EnumMember(Value = "RECTANGLE")]
    Rectangle,
    [EnumMember(Value = "ROUND")]
    Round
}