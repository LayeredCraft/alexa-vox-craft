using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR.Pipeline;

/// <summary>
/// Defines a handler for exceptions that occur during request processing.
/// Exception handlers provide a way to gracefully handle errors and return
/// appropriate responses to users when unexpected errors occur in skill processing.
/// </summary>
public interface IExceptionHandler
{
    /// <summary>
    /// Determines whether this exception handler can process the given exception.
    /// This allows for specific handlers to handle different types of exceptions
    /// or exceptions that occur in specific contexts.
    /// </summary>
    /// <param name="handlerInput">The handler input containing the request and utilities.</param>
    /// <param name="ex">The exception that occurred during request processing.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if this handler can process the exception; otherwise, false.</returns>
    Task<bool> CanHandle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the exception and generates an appropriate skill response.
    /// This method is called after CanHandle returns true for this exception handler.
    /// </summary>
    /// <param name="handlerInput">The handler input containing the request and utilities.</param>
    /// <param name="ex">The exception that occurred during request processing.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the skill response to send to the user.</returns>
    Task<SkillResponse> Handle(IHandlerInput handlerInput, Exception ex, CancellationToken cancellationToken = default);
}