using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Lambda.Context;

/// <summary>
/// Factory for creating and managing skill contexts within Lambda functions.
/// The skill context provides request-scoped data and utilities that are
/// accessible throughout the request processing pipeline.
/// </summary>
public interface ISkillContextFactory
{
    /// <summary>
    /// Creates a new skill context for the given request.
    /// This context will be available to request handlers and other components
    /// during the processing of this specific request.
    /// </summary>
    /// <param name="request">The Alexa skill request to create a context for.</param>
    /// <returns>A new skill context instance.</returns>
    SkillContext Create(SkillRequest request);

    /// <summary>
    /// Disposes the specified skill context and performs any necessary cleanup.
    /// This method should be called when request processing is complete.
    /// </summary>
    /// <param name="skillContext">The skill context to dispose.</param>
    void Dispose(SkillContext skillContext);
}