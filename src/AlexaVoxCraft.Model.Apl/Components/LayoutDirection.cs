using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<LayoutDirection>))]
public enum LayoutDirection
{
    [EnumMember(Value="inherit")]
    Inherit,
    [EnumMember(Value="LTR")]
    LTR,
    [EnumMember(Value = "RTL")]
    RTL
}