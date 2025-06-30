using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<ImportType>))]
public enum ImportType
{
    [EnumMember(Value="package")]
    Package,
    [EnumMember(Value = "oneOf")]
    OneOf,
    [EnumMember(Value = "allOf")]
    AllOf,
}