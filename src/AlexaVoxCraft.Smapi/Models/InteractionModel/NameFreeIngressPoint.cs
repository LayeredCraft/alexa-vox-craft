using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.InteractionModel;

/// <summary>
/// Describes a single ingress point for name-free interaction, such as
/// a skill launch or a specific intent.
/// </summary>
public sealed record NameFreeIngressPoint
{
    /// <summary>
    /// The ingress type. Typically "LAUNCH" or "INTENT".
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The intent name for INTENT ingress points. Null for LAUNCH ingress points.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Sample utterances that act as name-free entry points for this ingress.
    /// </summary>
    [JsonPropertyName("sampleUtterances")]
    public IReadOnlyList<NameFreeSampleUtterance> SampleUtterances { get; init; } = [];
}