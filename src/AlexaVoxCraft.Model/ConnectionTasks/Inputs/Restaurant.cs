using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class Restaurant
{
    [JsonPropertyName("@type")]
    public string Type => "Restaurant";

    [JsonPropertyName("@version")]
    public string Version => 1.ToString();

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("location")]
    public PostalAddress Location { get; set; }
}