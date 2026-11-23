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

    /// <summary>Gets the message payload; <see cref="JsonValueKind.Undefined"/> when the request had no message.</summary>
    [JsonPropertyName("message")]
    public JsonElement Message => throw new NotImplementedException();

    /// <summary>Deserializes the message payload using the caller-supplied metadata.</summary>
    /// <typeparam name="TMessage">The payload type.</typeparam>
    /// <param name="typeInfo">The source-generated metadata for <typeparamref name="TMessage"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown when the request carried no message.</exception>
    public TMessage? GetMessage<TMessage>(JsonTypeInfo<TMessage> typeInfo) => throw new NotImplementedException();
}
