using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents the body of the Alexa interaction model.
/// </summary>
public sealed record InteractionModelBody
{
    /// <summary>
    /// Gets the language model for the skill.
    /// </summary>
    [JsonPropertyName("languageModel")]
    public LanguageModel LanguageModel { get; init; } = default!;
    
    /// <summary>
    /// Optional Name-Free Interaction (NFI) configuration for the skill.
    /// Enables launch and intent ingress points that don't require the skill name.
    /// </summary>
    [JsonPropertyName("_nameFreeInteraction")]
    public NameFreeInteraction? NameFreeInteraction { get; init; }
}