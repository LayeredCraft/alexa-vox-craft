using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class Device
{
    [JsonPropertyName("deviceId")]
    public string DeviceID { get; set; }

    [JsonPropertyName("supportedInterfaces")]
    public Dictionary<string, object> SupportedInterfaces { get; set; }

    public bool IsInterfaceSupported(string interfaceName)
    {
        var hasInterface = SupportedInterfaces?.ContainsKey(interfaceName);
        return (hasInterface.HasValue ? hasInterface.Value : false); 
    }

    [JsonPropertyName("persistentEndpointId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PersistentEndpointID { get; set; }
}