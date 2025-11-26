using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents a custom slot type.
/// </summary>
public sealed record SlotType
{
    /// <summary>
    /// Gets the slot type name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the values for the slot type.
    /// </summary>
    [JsonPropertyName("values")]
    public IReadOnlyList<SlotTypeValue> Values { get; init; } = [];
}