using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR;

/// <summary>
/// Defines the contract for handling Alexa skill requests. Request handlers are responsible
/// for processing specific types of requests and generating appropriate responses.
/// </summary>
public interface IRequestHandler
{
    /// <summary>
    /// Determines whether this handler can process the given request.
    /// This method is used by the mediator to route requests to appropriate handlers.
    /// </summary>
    /// <param name="input">The handler input containing the request and utilities.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if this handler can process the request; otherwise, false.</returns>
    Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles the Alexa skill request and generates an appropriate response.
    /// This method is called after CanHandle returns true for this handler.
    /// </summary>
    /// <param name="input">The handler input containing the request and utilities.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the skill response.</returns>
    Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a default request handler that can handle any request type.
/// This interface is typically used for fallback handlers that should always be available
/// to process requests when no other specific handler is found.
/// </summary>
public interface IDefaultRequestHandler : IRequestHandler
{
    /// <summary>
    /// Always returns true, indicating this handler can process any request as a fallback.
    /// </summary>
    /// <param name="input">The handler input containing the request and utilities.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that always resolves to true.</returns>
    public new Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(true);
}

/// <summary>
/// Defines a strongly-typed request handler for a specific Alexa request type.
/// This interface provides compile-time safety by constraining the handler to work
/// with a specific request type.
/// </summary>
/// <typeparam name="TRequestType">The specific type of Alexa request this handler processes. Must inherit from Request.</typeparam>
public interface IRequestHandler<TRequestType> : IRequestHandler where TRequestType : Request
{
}
