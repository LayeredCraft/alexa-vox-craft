using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.InSkillPurchasing;
using AlexaVoxCraft.Model.InSkillPurchasing.Responses;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class CancelResponseHandler : IRequestHandler<ConnectionResponseRequest<ConnectionResponsePayload>>
{
    private readonly IInSkillPurchasingClient _ispClient;
    private readonly ILogger<CancelResponseHandler> _logger;

    public CancelResponseHandler(IInSkillPurchasingClient ispClient, ILogger<CancelResponseHandler> logger)
    {
        _ispClient = ispClient ?? throw new ArgumentNullException(nameof(ispClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is ConnectionResponseRequest<ConnectionResponsePayload>
        {
            Name: PaymentType.Cancel
        });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(CancelResponseHandler), nameof(Handle));

        var request = (ConnectionResponseRequest<ConnectionResponsePayload>)input.RequestEnvelope.Request;

        if (request.Status.Code != 200)
        {
            _logger.Error("Connections.Response indicated failure: {Message}", request.Status.Message);
            return await input.ResponseBuilder
                .Speak("There was an error handling your cancellation request. Please try again or contact us for help.")
                .GetResponse(cancellationToken);
        }

        var yesNoQuestion = FactHelpers.GetRandomYesNoQuestion();

        if (request.Payload.PurchaseResult == "ACCEPTED")
        {
            return await input.ResponseBuilder
                .Speak($"You have successfully cancelled your subscription. {yesNoQuestion}")
                .Reprompt(yesNoQuestion)
                .GetResponse(cancellationToken);
        }

        if (request.Payload.PurchaseResult == "NOT_ENTITLED")
        {
            return await input.ResponseBuilder
                .Speak($"You don't currently have a subscription to cancel. {yesNoQuestion}")
                .Reprompt(yesNoQuestion)
                .GetResponse(cancellationToken);
        }

        _logger.Warning("Unhandled cancellation result: {PurchaseResult}", request.Payload.PurchaseResult);
        return await input.ResponseBuilder
            .Speak("There was an error handling your cancellation request. Please try again or contact us for help.")
            .GetResponse(cancellationToken);
    }
}