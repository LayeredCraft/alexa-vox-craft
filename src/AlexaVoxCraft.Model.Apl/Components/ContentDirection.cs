using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Components;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ContentDirection>))]
public enum ContentDirection
{
    [EnumMember(Value="column")]
    Column,
    [EnumMember(Value="row")]
    Row
}