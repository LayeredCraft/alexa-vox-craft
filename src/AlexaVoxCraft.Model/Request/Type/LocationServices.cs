using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Request.Type;

public class LocationServices
{
    [JsonPropertyName("access")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<LocationServiceAccess>))]
    public LocationServiceAccess Access { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<LocationServiceStatus>))]
    public LocationServiceStatus Status { get; set; }
}