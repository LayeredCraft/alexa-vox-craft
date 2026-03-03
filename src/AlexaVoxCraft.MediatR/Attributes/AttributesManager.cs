using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Attributes;

/// <summary>
/// Default implementation of <see cref="IAttributesManager"/> that manages session, request,
/// and persistent attributes for a single Alexa skill request lifecycle.
/// </summary>
public class AttributesManager : IAttributesManager
{
    private readonly SkillRequest _eventRequest;
    private readonly IPersistenceAdapter? _persistenceAdapter;
    private bool _persistentLoaded;
    private Dictionary<string, JsonElement> _persistentAttributes = [];
    private JsonAttributeBag? _persistentBag;

    /// <inheritdoc/>
    public JsonAttributeBag Session { get; }

    /// <inheritdoc/>
    public JsonAttributeBag Request { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="AttributesManager"/>.
    /// </summary>
    /// <param name="skillRequestFactory">A factory delegate that returns the current <see cref="SkillRequest"/>.</param>
    /// <param name="persistenceAdapter">
    /// An optional persistence adapter for loading and saving persistent attributes.
    /// Required only when calling <see cref="GetPersistentAsync"/> or <see cref="SavePersistentAttributes"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="skillRequestFactory"/> is <see langword="null"/> or returns <see langword="null"/>.
    /// </exception>
    public AttributesManager(SkillRequestFactory skillRequestFactory, IPersistenceAdapter? persistenceAdapter = null)
    {
        ArgumentNullException.ThrowIfNull(skillRequestFactory);
        _persistenceAdapter = persistenceAdapter;
        _eventRequest = skillRequestFactory() ?? throw new ArgumentNullException(nameof(_eventRequest));

        var sessionDict = _eventRequest.Session?.Attributes ?? new Dictionary<string, JsonElement>();
        Session = new JsonAttributeBag(sessionDict);
        Request = new JsonAttributeBag(new Dictionary<string, JsonElement>());
    }

    /// <inheritdoc/>
    public async Task<JsonAttributeBag> GetPersistentAsync(CancellationToken ct = default)
    {
        if (_persistenceAdapter is null)
            throw new InvalidOperationException(
                $"{nameof(IPersistenceAdapter)} is required but was not registered.");

        if (!_persistentLoaded)
        {
            _persistentAttributes = (await _persistenceAdapter.GetAttributes(_eventRequest, ct)).ToDictionary();
            _persistentBag = new JsonAttributeBag(_persistentAttributes);
            _persistentLoaded = true;
        }

        return _persistentBag!;
    }

    /// <inheritdoc/>
    public async Task SavePersistentAttributes(CancellationToken cancellationToken = default)
    {
        if (_persistenceAdapter is null)
            throw new InvalidOperationException(
                $"{nameof(IPersistenceAdapter)} is required but was not registered.");

        if (_persistentLoaded)
        {
            await _persistenceAdapter.SaveAttribute(_eventRequest, _persistentAttributes, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public Task<Session?> GetSession(CancellationToken cancellationToken = default) =>
        Task.FromResult(_eventRequest.Session);
}