using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes;

/// <summary>
/// Manages session, request, and persistent attributes for a single Alexa skill request lifecycle.
/// </summary>
public interface IAttributesManager
{
    /// <summary>
    /// Gets the session-scoped attribute bag, backed by <see cref="Session.Attributes"/>.
    /// Changes made to this bag are included in the outgoing response automatically.
    /// </summary>
    JsonAttributeBag Session { get; }

    /// <summary>
    /// Gets the request-scoped attribute bag, valid only for the duration of the current request.
    /// </summary>
    JsonAttributeBag Request { get; }

    /// <summary>
    /// Loads persistent attributes from the configured persistence adapter on the first call,
    /// then returns the same cached <see cref="JsonAttributeBag"/> on subsequent calls.
    /// </summary>
    /// <param name="ct">A token to cancel the asynchronous operation.</param>
    /// <returns>The persistent attribute bag.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no persistence adapter has been registered.
    /// </exception>
    Task<JsonAttributeBag> GetPersistentAsync(CancellationToken ct = default);

    /// <summary>
    /// Persists the previously loaded persistent attributes via the configured persistence adapter.
    /// Does nothing if <see cref="GetPersistentAsync"/> has not been called first.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no persistence adapter has been registered.
    /// </exception>
    Task SavePersistentAttributes(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the <see cref="Session"/> from the current skill request, or <see langword="null"/>
    /// for sessionless request types such as audio player and playback controller requests.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    Task<Session?> GetSession(CancellationToken cancellationToken = default);
}