using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// Inbound request the web app sends to the skill as an <c>Alexa.Presentation.HTML.Message</c>.
/// The <see cref="Message"/> payload is preserved exactly and never deserialized into an arbitrary CLR type.
/// </summary>
public sealed class MessageRequest : AlexaVoxCraft.Model.Request.Type.Request
{
    /// <summary>The request type discriminator value.</summary>
    public const string RequestType = "Alexa.Presentation.HTML.Message";

    private JsonElement _message;

    /// <summary>
    /// Gets or sets the message payload; <see cref="JsonValueKind.Undefined"/> when the request had no
    /// message. The element is cloned on assignment so it remains valid after the source document is disposed.
    /// </summary>
    [JsonPropertyName("message")]
    public JsonElement Message
    {
        get => _message;
        set => _message = value.ValueKind == JsonValueKind.Undefined ? value : value.Clone();
    }

    /// <summary>Deserializes the message payload using the caller-supplied metadata.</summary>
    /// <typeparam name="TMessage">The payload type.</typeparam>
    /// <param name="typeInfo">The source-generated metadata for <typeparamref name="TMessage"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown when the request carried no message.</exception>
    /// <exception cref="JsonException">Thrown when the payload does not match <typeparamref name="TMessage"/>.</exception>
    public TMessage? GetMessage<TMessage>(JsonTypeInfo<TMessage> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        if (Message.ValueKind == JsonValueKind.Undefined)
        {
            throw new InvalidOperationException("The message request did not carry a message payload.");
        }

        return Message.Deserialize(typeInfo);
    }
}
