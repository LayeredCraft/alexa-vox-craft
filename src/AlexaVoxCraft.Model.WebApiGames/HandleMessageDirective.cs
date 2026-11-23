using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// Directive that delivers a message from the skill to a running web app. Serialized as an
/// <c>Alexa.Presentation.HTML.HandleMessage</c> directive.
/// </summary>
public sealed class HandleMessageDirective : IDirective
{
    /// <summary>The directive type discriminator value.</summary>
    public const string DirectiveType = "Alexa.Presentation.HTML.HandleMessage";

    /// <summary>Initializes a new handle-message directive.</summary>
    /// <param name="message">The message payload; must be a JSON object.</param>
    /// <param name="transformers">Optional transformers applied to the document.</param>
    public HandleMessageDirective(JsonElement message, IEnumerable<HTMLTransformer>? transformers = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type => throw new NotImplementedException();

    /// <summary>Gets the message payload.</summary>
    [JsonPropertyName("message")]
    public JsonElement Message => throw new NotImplementedException();

    /// <summary>Gets the optional transformers; omitted from the wire when null.</summary>
    [JsonPropertyName("transformers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<HTMLTransformer>? Transformers => throw new NotImplementedException();

    /// <summary>Creates a handle-message directive whose payload is serialized with the caller-supplied metadata.</summary>
    /// <typeparam name="TMessage">The message payload type.</typeparam>
    /// <param name="message">The message to deliver.</param>
    /// <param name="messageTypeInfo">The source-generated metadata for <typeparamref name="TMessage"/>.</param>
    /// <param name="transformers">Optional transformers applied to the document.</param>
    public static HandleMessageDirective Create<TMessage>(
        TMessage message,
        JsonTypeInfo<TMessage> messageTypeInfo,
        IEnumerable<HTMLTransformer>? transformers = null) => throw new NotImplementedException();
}
