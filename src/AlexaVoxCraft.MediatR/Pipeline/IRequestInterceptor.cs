namespace AlexaVoxCraft.MediatR.Pipeline;

/// <summary>
/// Defines an interceptor that processes requests before they reach the main request handler.
/// Request interceptors are useful for cross-cutting concerns such as logging, authentication,
/// validation, or request enrichment that should happen for all or most requests.
/// </summary>
public interface IRequestInterceptor
{
    /// <summary>
    /// Processes the incoming request before it reaches the main handler.
    /// This method can perform logging, validation, request modification, or other
    /// pre-processing operations.
    /// </summary>
    /// <param name="input">The handler input containing the request and utilities.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous processing operation.</returns>
    Task Process(IHandlerInput input, CancellationToken cancellationToken = default);
}