using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes;

public class AttributesManager : IAttributesManager
{
    private readonly SkillRequest _eventRequest;
    private readonly IPersistenceAdapter? _persistenceAdapter;
    private bool _persistentLoaded;
    private Dictionary<string, JsonElement> _persistentAttributes = [];

    public JsonAttributeBag Session { get; }
    public JsonAttributeBag Request { get; }

    public AttributesManager(SkillRequestFactory skillRequestFactory, IPersistenceAdapter? persistenceAdapter = null)
    {
        ArgumentNullException.ThrowIfNull(skillRequestFactory);
        _persistenceAdapter = persistenceAdapter;
        _eventRequest = skillRequestFactory() ?? throw new ArgumentNullException(nameof(_eventRequest));

        var sessionDict = _eventRequest.Session?.Attributes ?? new Dictionary<string, JsonElement>();
        Session = new JsonAttributeBag(sessionDict);
        Request = new JsonAttributeBag(new Dictionary<string, JsonElement>());
    }

    public async Task<JsonAttributeBag> GetPersistentAsync(CancellationToken ct = default)
    {
        if (_persistenceAdapter is null)
            throw new MissingMemberException(nameof(AttributesManager), nameof(_persistenceAdapter));

        if (!_persistentLoaded)
        {
            _persistentAttributes = (await _persistenceAdapter.GetAttributes(_eventRequest, ct)).ToDictionary();
            _persistentLoaded = true;
        }

        return new JsonAttributeBag(_persistentAttributes);
    }

    public async Task SavePersistentAttributes(CancellationToken cancellationToken = default)
    {
        if (_persistenceAdapter is null)
            throw new MissingMemberException(nameof(AttributesManager), nameof(_persistenceAdapter));

        if (_persistentLoaded)
        {
            await _persistenceAdapter.SaveAttribute(_eventRequest, _persistentAttributes!, cancellationToken);
        }
    }

    public Task<Session> GetSession(CancellationToken cancellationToken = default) =>
        Task.FromResult(_eventRequest.Session);

    public bool TryGetSessionState<TState>(string key, out TState? state)
        => Session.TryGet(key, out state);

    public TState? GetSessionState<TState>(string key)
        => Session.Get<TState>(key);

    public void SetSessionState<TState>(string key, TState state)
        => Session.Set(key, state);

    public void ClearSessionState(string key)
        => Session.Remove(key);
}