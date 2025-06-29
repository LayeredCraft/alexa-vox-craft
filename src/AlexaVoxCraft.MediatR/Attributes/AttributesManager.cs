using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes;

public class AttributesManager : IAttributesManager
{
    private readonly SkillRequest _eventRequest;
    private IDictionary<string, object> _requestAttributes = new Dictionary<string, object>();
    private IDictionary<string, object> _persistentAttributes = new Dictionary<string, object>();
    private IDictionary<string, object>? _sessionAttributes;
    private readonly IPersistenceAdapter? _persistenceAdapter;
    private bool _persistentAttributeSet = false;

    public AttributesManager(SkillRequestFactory skillRequestFactory, IPersistenceAdapter? persistenceAdapter = null)
    {
        ArgumentNullException.ThrowIfNull(skillRequestFactory);
        _persistenceAdapter = persistenceAdapter;
        _eventRequest = skillRequestFactory() ?? throw new ArgumentNullException(nameof(_eventRequest));
        if (_eventRequest.Session is not null)
            _sessionAttributes = _eventRequest.Session.Attributes ?? new Dictionary<string, object>();
    }

    public Task<IDictionary<string, object>> GetRequestAttributes(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_requestAttributes);
    }

    public Task<IDictionary<string, object>> GetSessionAttributes(CancellationToken cancellationToken = default)
    {
        if (_sessionAttributes is null)
            throw new MissingMemberException(nameof(SkillRequest), nameof(SkillRequest.Session));

        return Task.FromResult(_sessionAttributes);
    }

    public async Task<IDictionary<string, object>> GetPersistentAttributes(
        CancellationToken cancellationToken = default)
    {
        if (_persistenceAdapter is null)
            throw new MissingMemberException(nameof(AttributesManager), nameof(_persistenceAdapter));

        if (_persistentAttributeSet) return _persistentAttributes;
        _persistentAttributes = await _persistenceAdapter.GetAttributes(_eventRequest, cancellationToken);
        _persistentAttributeSet = true;

        return _persistentAttributes;
    }

    public Task SetRequestAttributes(IDictionary<string, object> requestAttributes,
        CancellationToken cancellationToken = default)
    {
        _requestAttributes = requestAttributes;

        return Task.CompletedTask;
    }

    public Task SetSessionAttributes(IDictionary<string, object> sessionAttributes,
        CancellationToken cancellationToken = default)
    {
        if (_sessionAttributes is null)
            throw new MissingMemberException(nameof(SkillRequest), nameof(SkillRequest.Session));

        _sessionAttributes = sessionAttributes;

        return Task.CompletedTask;
    }

    public Task SetPersistentAttributes(IDictionary<string, object> persistentAttributes,
        CancellationToken cancellationToken = default)
    {
        if (_persistenceAdapter is null)
            throw new MissingMemberException(nameof(AttributesManager), nameof(_persistenceAdapter));

        _persistentAttributes = persistentAttributes;
        _persistentAttributeSet = true;

        return Task.CompletedTask;
    }

    public async Task SavePersistentAttributes(CancellationToken cancellationToken = default)
    {
        if (_persistenceAdapter is null)
            throw new MissingMemberException(nameof(AttributesManager), nameof(_persistenceAdapter));

        if (_persistentAttributeSet)
        {
            await _persistenceAdapter.SaveAttribute(_eventRequest, _persistentAttributes, cancellationToken);
        }
    }

    public async Task<Session> GetSession(CancellationToken cancellationToken = default)
    {
        var attributes = await GetSessionAttributes(cancellationToken);
        var session = _eventRequest.Session;
        session!.Attributes = attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return session;
    }
}