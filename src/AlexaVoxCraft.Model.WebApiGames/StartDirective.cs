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
    public StartDirective(
        StartRequest request,
        StartConfiguration? configuration = null,
        JsonElement? data = null,
        IEnumerable<HTMLTransformer>? transformers = null) => throw new NotImplementedException();

    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type => throw new NotImplementedException();

    /// <summary>Gets the resource the web app loads.</summary>
    [JsonPropertyName("request")]
    public StartRequest Request => throw new NotImplementedException();

    /// <summary>Gets the start configuration; always present on the wire.</summary>
    [JsonPropertyName("configuration")]
    public StartConfiguration Configuration => throw new NotImplementedException();

    /// <summary>Gets the optional initial data; omitted from the wire when null.</summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Data => throw new NotImplementedException();

    /// <summary>Gets the optional transformers; omitted from the wire when null.</summary>
    [JsonPropertyName("transformers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<HTMLTransformer>? Transformers => throw new NotImplementedException();

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
        IEnumerable<HTMLTransformer>? transformers = null) => throw new NotImplementedException();
}
