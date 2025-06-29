using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<Snap>))]
public enum Snap
{
    [EnumMember(Value="none")]
    None,
    [EnumMember(Value="start")]
    Start,
    [EnumMember(Value="center")]
    Center,
    [EnumMember(Value="end")]
    End,
    [EnumMember(Value="forceStart")]
    ForceStart,
    [EnumMember(Value="forceCenter")]
    ForceCenter,
    [EnumMember(Value="forceEnd")]
    ForceEnd
}