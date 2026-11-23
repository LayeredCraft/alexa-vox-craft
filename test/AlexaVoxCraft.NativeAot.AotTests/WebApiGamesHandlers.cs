using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.WebApiGames;

namespace AlexaVoxCraft.NativeAot.AotTests;

/// <summary>
/// Web API for Games dispatch target for the Native AOT suite (Issue #213). Declared in this assembly so
/// the MediatR interceptor generator registers it, and the keyed wrapper for <see cref="MessageRequest"/>,
/// exactly as a real skill does. The inbound message is read back as a typed payload through the
/// caller-supplied source-generated <see cref="ValidationAppContext"/> metadata (no reflection), and the
/// response carries a <see cref="HandleMessageDirective"/> built from that typed payload.
/// </summary>
public sealed class WebApiGamesMessageHandler : IRequestHandler<MessageRequest>
{
    /// <inheritdoc />
    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is MessageRequest);

    /// <inheritdoc />
    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        var request = (MessageRequest)input.RequestEnvelope.Request;
        var message = request.GetMessage(ValidationAppContext.Default.GameMessage)
            ?? throw new InvalidOperationException("The handle-message request carried no message payload.");

        var directive = HandleMessageDirective.Create(
            new GameMessage { Action = message.Action, Level = message.Level },
            ValidationAppContext.Default.GameMessage);

        return await input.ResponseBuilder
            .AddDirective(directive)
            .GetResponse(cancellationToken);
    }
}
