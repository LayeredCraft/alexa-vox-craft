using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// Directive that starts the web app for a game. Serialized as an
/// <c>Alexa.Presentation.HTML.Start</c> directive.
/// </summary>
public sealed class StartDirective : IDirective
{
    /// <summary>The directive type discriminator value.</summary>
    public const string DirectiveType = "Alexa.Presentation.HTML.Start";

    /// <summary>Initializes a new start directive.</summary>
    /// <param name="request">The resource the web app loads.</param>
    /// <param name="configuration">The start configuration; defaults to a 30 second timeout when null.</param>
    /// <param name="data">Optional initial data for the web app; must be a JSON object when present.</param>
    /// <param name="transformers">Optional transformers applied to the document.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="data"/> is present but not a JSON object.</exception>
    public StartDirective(
        StartRequest request,
        StartConfiguration? configuration = null,
        JsonElement? data = null,
        IEnumerable<HTMLTransformer>? transformers = null)
        : this(request, configuration, data, Materialize(transformers))
    {
    }

    /// <summary>Initializes a new start directive from an already-materialized transformer list.</summary>
    /// <remarks>
    /// Deserialization entry point. System.Text.Json's parameterized-constructor binding requires the
    /// parameter types to match the property types exactly, so this constructor takes
    /// <see cref="IReadOnlyList{T}"/> rather than the public constructor's <see cref="IEnumerable{T}"/>.
    /// </remarks>
    [JsonConstructor]
    public StartDirective(
        StartRequest request,
        StartConfiguration? configuration,
        JsonElement? data,
        IReadOnlyList<HTMLTransformer>? transformers)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (data is { ValueKind: not JsonValueKind.Object })
        {
            throw new ArgumentException("The start directive data payload must be a JSON object.", nameof(data));
        }

        Request = request;
        Configuration = configuration ?? new StartConfiguration();
        Data = data?.Clone();
        Transformers = transformers is { Count: > 0 } ? transformers : null;
    }

    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    /// <summary>Gets the resource the web app loads.</summary>
    [JsonPropertyName("request")]
    public StartRequest Request { get; }

    /// <summary>Gets the start configuration; always present on the wire.</summary>
    [JsonPropertyName("configuration")]
    public StartConfiguration Configuration { get; }

    /// <summary>Gets the optional initial data; omitted from the wire when null.</summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Data { get; }

    /// <summary>Gets the optional transformers; omitted from the wire when null.</summary>
    [JsonPropertyName("transformers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<HTMLTransformer>? Transformers { get; }

    /// <summary>Creates a start directive whose data payload is serialized with the caller-supplied metadata.</summary>
    /// <typeparam name="TData">The payload type.</typeparam>
    /// <param name="request">The resource the web app loads.</param>
    /// <param name="data">The initial data for the web app.</param>
    /// <param name="dataTypeInfo">The source-generated metadata for <typeparamref name="TData"/>.</param>
    /// <param name="configuration">The start configuration; defaults to a 30 second timeout when null.</param>
    /// <param name="transformers">Optional transformers applied to the document.</param>
    public static StartDirective Create<TData>(
        StartRequest request,
        TData data,
        JsonTypeInfo<TData> dataTypeInfo,
        StartConfiguration? configuration = null,
        IEnumerable<HTMLTransformer>? transformers = null)
    {
        ArgumentNullException.ThrowIfNull(dataTypeInfo);

        var element = JsonSerializer.SerializeToElement(data, dataTypeInfo);
        return new StartDirective(request, configuration, element, transformers);
    }

    private static IReadOnlyList<HTMLTransformer>? Materialize(IEnumerable<HTMLTransformer>? transformers)
    {
        if (transformers is null)
        {
            return null;
        }

        var materialized = transformers.ToArray();
        return materialized.Length > 0 ? materialized : null;
    }
}
