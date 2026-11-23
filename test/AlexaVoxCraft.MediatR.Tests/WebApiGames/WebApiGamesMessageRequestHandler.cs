using System.Text.Json.Serialization;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.WebApiGames;

namespace AlexaVoxCraft.MediatR.Tests.WebApiGames;

/// <summary>
/// Ordinary generic <see cref="IRequestHandler{TRequestType}"/> for a Web API for Games
/// <see cref="MessageRequest"/>. Declared in this assembly so the AlexaVoxCraft MediatR interceptor
/// generator discovers it and emits the same keyed <c>AddKeyedSingleton</c> dispatch registration a real
/// skill gets from <c>AddSkillMediator(configuration, ...)</c>.
///
/// The inbound message is read back as a typed payload through <see cref="MessageRequest.GetMessage{TMessage}"/>
/// (caller-supplied source-generated metadata, no reflection) and drives which of the two Web API for
/// Games directives is added through the production response builder.
/// </summary>
public sealed class WebApiGamesMessageRequestHandler : IRequestHandler<MessageRequest>
{
    /// <summary>The web app resource the Start directive loads.</summary>
    public static readonly Uri GameUri = new("https://example.com/game");

    /// <inheritdoc />
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is MessageRequest);

    /// <inheritdoc />
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = (MessageRequest)input.RequestEnvelope.Request;
        var payload = request.GetMessage(WebApiGamesMessageJsonContext.Default.MessagePayload)
            ?? throw new InvalidOperationException("The handle-message request carried no message payload.");

        if (payload.Action == "start")
        {
            return await input.ResponseBuilder
                .AddDirective(new StartDirective(new StartRequest(GameUri)))
                .GetResponse(cancellationToken);
        }

        var handleMessage = HandleMessageDirective.Create(
            payload,
            WebApiGamesMessageJsonContext.Default.MessagePayload);

        return await input.ResponseBuilder
            .AddDirective(handleMessage)
            .GetResponse(cancellationToken);
    }
}

/// <summary>The typed payload of an inbound <see cref="MessageRequest"/>.</summary>
public sealed class MessagePayload
{
    /// <summary>Gets the action the web app requested.</summary>
    [JsonPropertyName("action")]
    public string Action { get; init; } = string.Empty;

    /// <summary>Gets the level carried by the message.</summary>
    [JsonPropertyName("level")]
    public int Level { get; init; }
}

/// <summary>
/// Caller-owned source-generated metadata for the handler's typed payload, proving
/// <see cref="MessageRequest.GetMessage{TMessage}"/> and the typed directive helpers need no reflection.
/// </summary>
[JsonSerializable(typeof(MessagePayload))]
internal partial class WebApiGamesMessageJsonContext : JsonSerializerContext
{
}
