using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ViewportTheme>))]
public enum ViewportTheme
{
    [EnumMember(Value="dark")]
    Dark,
    [EnumMember(Value="light")]
    Light
}