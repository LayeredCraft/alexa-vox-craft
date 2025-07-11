using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Pipeline;

/// <summary>
/// Represents a delegate that executes the next handler in the request processing pipeline.
/// </summary>
/// <returns>A task that represents the asynchronous operation. The task result contains the skill response.</returns>
public delegate Task<SkillResponse> RequestHandlerDelegate();

/// <summary>
/// Defines a behavior that can be injected into the request processing pipeline.
/// Pipeline behaviors allow for cross-cutting concerns such as logging, validation,
/// caching, and exception handling to be applied consistently across all request handlers.
/// </summary>
public interface IPipelineBehavior
{
    /// <summary>
    /// Handles the request within the processing pipeline. This method can perform
    /// pre-processing, call the next handler in the pipeline, and perform post-processing.
    /// </summary>
    /// <param name="input">The handler input containing the request and utilities.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <param name="next">A delegate to call the next handler in the pipeline.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the skill response.</returns>
    Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken, RequestHandlerDelegate next);
}