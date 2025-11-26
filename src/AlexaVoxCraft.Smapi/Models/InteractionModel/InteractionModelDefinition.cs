using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents the top-level Alexa interaction model definition.
/// </summary>
public sealed record InteractionModelDefinition
{
    /// <summary>
    /// Gets the semantic version of the interaction model.
    /// This field is required when sending an update to SMAPI.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = "1";

    /// <summary>
    /// Gets the description of the interaction model version.
    /// This field is required when sending an update to SMAPI.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the interaction model body.
    /// </summary>
    [JsonPropertyName("interactionModel")]
    public InteractionModelBody InteractionModel { get; init; } = default!;
}