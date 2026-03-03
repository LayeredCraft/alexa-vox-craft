using System.Text.Json;

namespace AlexaVoxCraft.Model.Request.Type;

/// <summary>
/// Resolves the concrete request type for <c>Connections.Response</c> requests by delegating to registered <see cref="IConnectionResponseHandler"/> instances.
/// </summary>
public class ConnectionResponseTypeResolver : IDataDrivenRequestTypeResolver
{
    private static volatile IReadOnlyList<IConnectionResponseHandler> _handlers = [new AskForRequestHandler()];
    private static readonly object RegisterLock = new();

    /// <summary>
    /// Registers a <see cref="IConnectionResponseHandler"/> for use during type resolution.
    /// Handlers are deduplicated by type; registering the same handler type twice has no effect.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    public static void Register(IConnectionResponseHandler handler)
    {
        lock (RegisterLock)
        {
            if (_handlers.Any(h => h.GetType() == handler.GetType()))
                return;

            _handlers = [.. _handlers, handler];
        }
    }

    /// <inheritdoc />
    public bool CanResolve(string requestType)
    {
        return requestType == "Connections.Response";
    }

    /// <inheritdoc />
    public System.Type? Resolve(string requestType)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public System.Type? Resolve(JsonElement data)
    {
        var handler = _handlers.FirstOrDefault(h => h.CanCreate(data));
        return handler?.Create(data);
    }
}