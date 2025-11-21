using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents an Alexa intent.
/// </summary>
public sealed record Intent
{
    /// <summary>
    /// Gets the intent name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the sample utterances for the intent.
    /// </summary>
    [JsonPropertyName("samples")]
    public IReadOnlyList<string> Samples { get; init; } = [];

    /// <summary>
    /// Gets the slots associated with the intent.
    /// </summary>
    [JsonPropertyName("slots")]
    public IReadOnlyList<IntentSlot> Slots { get; init; } = [];
}