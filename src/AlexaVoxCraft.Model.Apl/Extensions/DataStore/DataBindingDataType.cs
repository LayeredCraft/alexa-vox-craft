using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.Extensions.DataStore;

[JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<DataBindingDataType>))]
public enum DataBindingDataType
{
    [EnumMember(Value="OBJECT")]
    Object,
    [EnumMember(Value="ARRAY")]
    Array
}