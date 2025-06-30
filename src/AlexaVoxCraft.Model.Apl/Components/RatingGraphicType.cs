using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<RatingGraphicType>))]
public enum RatingGraphicType
{
    [EnumMember(Value="AVG")]
    AVG,
    [EnumMember(Value="image")]
    Image
}