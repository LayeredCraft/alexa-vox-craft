using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Smapi.Models.Invocation;

/// <summary>
/// Represents a request to the SMAPI Skill Invocation API.
/// </summary>
/// <typeparam name="TSkillRequest">The Alexa skill request envelope type.</typeparam>
public sealed record SkillInvocationRequest<TSkillRequest>
{
    /// <summary>
    /// Gets or sets the endpoint region to invoke.
    /// </summary>
    [JsonPropertyName("endpointRegion")]
    public InvocationRegion EndpointRegion { get; init; } = InvocationRegion.Default;

    /// <summary>
    /// Gets or sets the Alexa skill request wrapper.
    /// </summary>
    [JsonPropertyName("skillRequest")]
    public SkillInvocationBody<TSkillRequest> SkillRequest { get; init; } = new();
}

/// <summary>
/// Wraps the Alexa request envelope sent to the skill endpoint.
/// </summary>
/// <typeparam name="TSkillRequest">The Alexa skill request envelope type.</typeparam>
public sealed record SkillInvocationBody<TSkillRequest>
{
    /// <summary>
    /// Gets or sets the Alexa skill request body.
    /// </summary>
    [JsonPropertyName("body")]
    public TSkillRequest Body { get; init; } = default!;
}

/// <summary>
/// Represents the endpoint region used for invocation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<InvocationRegion>))]
public enum InvocationRegion
{
    Default,
    NA,
    EU,
    FE
}