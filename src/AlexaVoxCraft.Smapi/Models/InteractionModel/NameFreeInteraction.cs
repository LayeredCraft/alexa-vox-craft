using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Represents the Name-Free Interaction (NFI) container in an Alexa interaction model.
/// </summary>
public sealed record NameFreeInteraction
{
    /// <summary>
    /// Ingress points that define how customers can enter the skill without using its name.
    /// </summary>
    [JsonPropertyName("ingressPoints")]
    public IReadOnlyList<NameFreeIngressPoint> IngressPoints { get; init; } = [];
}