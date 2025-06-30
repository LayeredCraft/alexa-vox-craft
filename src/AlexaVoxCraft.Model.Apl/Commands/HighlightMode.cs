using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Commands;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<HighlightMode>))]
public enum HighlightMode
{
    [EnumMember(Value="line")]
    Line,
    [EnumMember(Value = "block")]
    Block
}