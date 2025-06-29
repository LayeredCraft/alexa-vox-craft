using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Package;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<InstallStateChanges>))]
public enum InstallStateChanges
{
    [EnumMember(Value="AUTOMATIC")]
    Automatic,
    [EnumMember(Value="INFORM")]
    Inform
}