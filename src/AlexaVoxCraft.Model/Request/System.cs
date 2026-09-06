using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

public class AlexaSystem
{
    [JsonPropertyName("apiAccessToken")]
    public string ApiAccessToken { get; set; } = null!;

    [JsonPropertyName("apiEndpoint")]
    public string ApiEndpoint { get; set; } = null!;

    [JsonPropertyName("application")]
    public Application Application { get; set; } = null!;

    [JsonPropertyName("person")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Person? Person { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; } = null!;

    [JsonPropertyName("device")]
    public Device Device { get; set; } = null!;

    [JsonPropertyName("unit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Unit? Unit { get; set; }
}