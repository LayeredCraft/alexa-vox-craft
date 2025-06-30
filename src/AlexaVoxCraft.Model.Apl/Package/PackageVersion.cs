using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Package;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<PackageVersion>))]
public enum PackageVersion
{
    [EnumMember(Value="1.0")]
    V1_0
}