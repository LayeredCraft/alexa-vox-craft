
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response;

public class StandardCard : ICard
{
    public const string CardType = "Standard";

    [JsonPropertyName("type")]
    public string Type => CardType;

    [JsonRequired]
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonRequired]
    [JsonPropertyName("text")]
    public string Content { get; set; } = null!;

    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CardImage Image { get; set; } = null!;
}