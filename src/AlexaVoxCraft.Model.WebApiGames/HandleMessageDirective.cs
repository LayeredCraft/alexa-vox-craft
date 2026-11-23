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
    /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is not a JSON object.</exception>
    public HandleMessageDirective(JsonElement message, IEnumerable<HTMLTransformer>? transformers = null)
        : this(message, Materialize(transformers))
    {
    }

    /// <summary>Initializes a new handle-message directive from an already-materialized transformer list.</summary>
    /// <remarks>
    /// Internal deserialization entry point. System.Text.Json's parameterized-constructor binding requires
    /// the parameter types to match the property types exactly, so this overload takes
    /// <see cref="IReadOnlyList{T}"/> rather than the <see cref="IEnumerable{T}"/> accepted by the public
    /// constructor.
    /// </remarks>
    [JsonConstructor]
    internal HandleMessageDirective(JsonElement message, IReadOnlyList<HTMLTransformer>? transformers)
    {
        if (message.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("The handle-message payload must be a JSON object.", nameof(message));
        }

        Message = message.Clone();
        Transformers = transformers is { Count: > 0 } ? transformers : null;
    }

    /// <inheritdoc />
    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    /// <summary>Gets the message payload.</summary>
    [JsonPropertyName("message")]
    public JsonElement Message { get; }

    /// <summary>Gets the optional transformers; omitted from the wire when null.</summary>
    [JsonPropertyName("transformers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<HTMLTransformer>? Transformers { get; }

    /// <summary>Creates a handle-message directive whose payload is serialized with the caller-supplied metadata.</summary>
    /// <typeparam name="TMessage">The message payload type.</typeparam>
    /// <param name="message">The message to deliver.</param>
    /// <param name="messageTypeInfo">The source-generated metadata for <typeparamref name="TMessage"/>.</param>
    /// <param name="transformers">Optional transformers applied to the document.</param>
    public static HandleMessageDirective Create<TMessage>(
        TMessage message,
        JsonTypeInfo<TMessage> messageTypeInfo,
        IEnumerable<HTMLTransformer>? transformers = null)
    {
        ArgumentNullException.ThrowIfNull(messageTypeInfo);

        var element = JsonSerializer.SerializeToElement(message, messageTypeInfo);
        return new HandleMessageDirective(element, transformers);
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
