using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response;

public class LinkAccountCard : ICard
{
    public const string CardType = "LinkAccount";

    [JsonPropertyName("type")]
    public string Type => CardType;
}