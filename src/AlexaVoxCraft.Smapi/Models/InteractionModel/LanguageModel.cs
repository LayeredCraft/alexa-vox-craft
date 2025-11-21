using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents the language model containing intents and slot types.
/// </summary>
public sealed record LanguageModel
{
    /// <summary>
    /// Gets the invocation name of the skill.
    /// </summary>
    [JsonPropertyName("invocationName")]
    public string InvocationName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the collection of intents.
    /// </summary>
    [JsonPropertyName("intents")]
    public IReadOnlyList<Intent> Intents { get; init; } = [];

    /// <summary>
    /// Gets the collection of custom slot types.
    /// </summary>
    [JsonPropertyName("types")]
    public IReadOnlyList<SlotType> Types { get; init; } = [];
}