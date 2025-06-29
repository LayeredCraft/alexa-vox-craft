using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Extensions.Backstack;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<BackTypeKind>))]
public enum BackTypeKind
{
    [EnumMember(Value="count")]
    Count,
    [EnumMember(Value = "index")]
    Index,
    [EnumMember(Value = "id")]
    Id
}