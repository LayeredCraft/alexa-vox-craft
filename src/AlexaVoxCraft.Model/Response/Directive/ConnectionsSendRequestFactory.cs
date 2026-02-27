using System.Text.Json;

namespace AlexaVoxCraft.Model.Response.Directive;

/// <summary>
/// Factory that resolves the concrete directive type for <c>Connections.SendRequest</c> directives
/// by delegating to registered <see cref="IConnectionSendRequestHandler"/> instances.
/// </summary>
public static class ConnectionSendRequestFactory
{
    private static volatile IReadOnlyList<IConnectionSendRequestHandler> _handlers = [new AskForPermissionDirectiveHandler()];
    private static readonly object RegisterLock = new();

    /// <summary>
    /// Registers a <see cref="IConnectionSendRequestHandler"/> for use during directive type resolution.
    /// Handlers are deduplicated by type; registering the same handler type twice has no effect.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    public static void Register(IConnectionSendRequestHandler handler)
    {
        lock (RegisterLock)
        {
            if (_handlers.Any(h => h.GetType() == handler.GetType()))
                return;

            _handlers = [.._handlers, handler];
        }
    }

    /// <summary>
    /// Resolves the concrete directive <see cref="Type"/> for the given <c>Connections.SendRequest</c> JSON element.
    /// </summary>
    /// <param name="data">The JSON element representing the directive.</param>
    /// <returns>The concrete <see cref="Type"/> to deserialize the directive into.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no registered handler can resolve the directive.</exception>
    public static Type Create(JsonElement data)
    {
        var handler = _handlers.FirstOrDefault(h => h.CanCreate(data));

        return handler == null
            ? throw new InvalidOperationException("Unable to parse Connections.SendRequest directive")
            : handler.Create();
    }
}