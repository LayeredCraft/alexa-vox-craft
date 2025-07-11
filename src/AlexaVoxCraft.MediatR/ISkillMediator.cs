using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR;

/// <summary>
/// Mediates the processing of Alexa skill requests by routing them to appropriate handlers.
/// This interface provides the main entry point for processing skill requests in a decoupled,
/// testable manner using the mediator pattern.
/// </summary>
public interface ISkillMediator
{
    /// <summary>
    /// Processes an Alexa skill request and returns the appropriate response.
    /// The mediator handles skill ID verification, request routing, and handler execution
    /// with proper error handling and logging.
    /// </summary>
    /// <param name="request">The Alexa skill request to process.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the skill response.</returns>
    /// <exception cref="ArgumentException">Thrown when skill ID verification fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no suitable handler can be found for the request type.</exception>
    Task<SkillResponse> Send(SkillRequest request, CancellationToken cancellationToken = default);
}