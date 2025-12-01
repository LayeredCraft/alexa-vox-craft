using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents a sample utterance for a name-free ingress point.
/// </summary>
public sealed record NameFreeSampleUtterance
{
    /// <summary>
    /// The utterance format. Currently "RAW_TEXT" for plain text phrases.
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; init; } = "RAW_TEXT";

    /// <summary>
    /// The utterance text value.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;
}