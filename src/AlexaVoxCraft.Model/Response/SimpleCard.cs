using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response;

public class SimpleCard : ICard
{
    public const string CardType = "Simple";

    [JsonPropertyName("type")]
    public string Type => CardType;

    [JsonPropertyName("title")]
    [JsonRequired]
    public string Title { get; set; } = null!;

    [JsonRequired]
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}