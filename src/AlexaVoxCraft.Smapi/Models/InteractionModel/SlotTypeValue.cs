using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents a value for a custom slot type.
/// </summary>
public sealed record SlotTypeValue
{
    /// <summary>
    /// Gets the value definition.
    /// </summary>
    [JsonPropertyName("name")]
    public SlotTypeValueName Name { get; init; } = default!;
}