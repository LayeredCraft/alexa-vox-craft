using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents the canonical name and synonyms for a slot type value.
/// </summary>
public sealed record SlotTypeValueName
{
    /// <summary>
    /// Gets the primary value name.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Gets the synonyms for the value.
    /// </summary>
    [JsonPropertyName("synonyms")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Synonyms { get; init; }
}