using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class PostalAddress
{
    [JsonPropertyName("@type")] public string Type => "PostalAddress";
    [JsonPropertyName("@version")] public string Version => 1.ToString();
    [JsonPropertyName("streetAddress")] public string StreetAddress { get; set; }
    [JsonPropertyName("locality")] public string Locality { get; set; }
    [JsonPropertyName("region")] public string Region { get; set; }
    [JsonPropertyName("postalCode")] public string PostalCode { get; set; }
    [JsonPropertyName("country")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Country { get; set; }
}