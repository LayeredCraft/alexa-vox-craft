using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class AlexaSystem
{
    [JsonPropertyName("apiAccessToken")]
    public string ApiAccessToken { get; set; }

    [JsonPropertyName("apiEndpoint")]
    public string ApiEndpoint { get; set; }

    [JsonPropertyName("application")]
    public Application Application { get; set; }

    [JsonPropertyName("person")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Person? Person { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }

    [JsonPropertyName("device")]
    public Device Device { get; set; }

    [JsonPropertyName("unit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Unit? Unit { get; set; }
}