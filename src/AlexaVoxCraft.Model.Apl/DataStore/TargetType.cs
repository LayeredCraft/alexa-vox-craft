using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.DataStore;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<TargetType>))]
public enum TargetType
{
    [EnumMember(Value="DEVICES")]
    Devices,
    [EnumMember(Value="USER")]
    User
}