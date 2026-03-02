using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes;

public interface IAttributesManager
{
    JsonAttributeBag Session { get; }
    JsonAttributeBag Request { get; }
    Task<JsonAttributeBag> GetPersistentAsync(CancellationToken ct = default);
    Task SavePersistentAttributes(CancellationToken cancellationToken = default);
    Task<Session> GetSession(CancellationToken cancellationToken = default);
    bool TryGetSessionState<TState>(string key, out TState? state);
    TState? GetSessionState<TState>(string key);
    void SetSessionState<TState>(string key, TState state);
    void ClearSessionState(string key);}