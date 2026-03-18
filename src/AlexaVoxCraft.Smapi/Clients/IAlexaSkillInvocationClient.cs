using AlexaVoxCraft.Smapi.Models.Invocation;

namespace AlexaVoxCraft.Smapi.Clients;

/// <summary>
/// Client for invoking Alexa skills through the SMAPI Skill Invocation API.
/// </summary>
public interface IAlexaSkillInvocationClient
{
    /// <summary>
    /// Invokes the specified skill for testing using an Alexa skill request envelope.
    /// </summary>
    /// <typeparam name="TSkillRequest">The Alexa skill request envelope type.</typeparam>
    /// <param name="skillId">The Alexa skill identifier.</param>
    /// <param name="stage">The skill stage.</param>
    /// <param name="skillRequest">The Alexa request envelope sent to the skill endpoint.</param>
    /// <param name="endpointRegion">The endpoint region to invoke.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The invocation response, or null if no content was returned.</returns>
    Task<SkillInvocationResponse<TResponse>?> InvokeAsync<TRequest, TResponse>(
        string skillId,
        string stage,
        TRequest skillRequest,
        InvocationRegion endpointRegion = InvocationRegion.Default,
        CancellationToken ct = default);
}