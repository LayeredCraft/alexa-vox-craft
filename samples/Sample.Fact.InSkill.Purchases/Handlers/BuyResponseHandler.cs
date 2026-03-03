using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.InSkillPurchasing;
using AlexaVoxCraft.Model.InSkillPurchasing.Responses;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Data;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class BuyResponseHandler : IRequestHandler<ConnectionResponseRequest<ConnectionResponsePayload>>
{
    private readonly IInSkillPurchasingClient _ispClient;
    private readonly ILogger<BuyResponseHandler> _logger;

    public BuyResponseHandler(IInSkillPurchasingClient ispClient, ILogger<BuyResponseHandler> logger)
    {
        _ispClient = ispClient ?? throw new ArgumentNullException(nameof(ispClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is ConnectionResponseRequest<ConnectionResponsePayload>
        {
            Name: PaymentType.Buy or PaymentType.Upsell
        });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(BuyResponseHandler), nameof(Handle));

        var request = (ConnectionResponseRequest<ConnectionResponsePayload>)input.RequestEnvelope.Request;

        if (request.Status.Code != 200)
        {
            _logger.Error("Connections.Response indicated failure: {Message}", request.Status.Message);
            return await input.ResponseBuilder
                .Speak("There was an error handling your purchase request. Please try again or contact us for help.")
                .GetResponse(cancellationToken);
        }

        var product = await _ispClient.GetProductAsync(request.Payload.ProductId, cancellationToken);

        input.AttributesManager.Session.TryGet<Product[]>("entitledProducts", out var entitledProducts);

        string speakOutput;
        string repromptOutput;

        switch (request.Payload.PurchaseResult)
        {
            case "ACCEPTED":
                var acceptedFacts = product?.ReferenceName != "all_access"
                    ? FactData.AllFacts.Where(f => f.Type == product?.ReferenceName.Replace("_pack", "")).ToArray()
                    : FactData.AllFacts;
                var categoryLabel = product?.ReferenceName.Replace("_pack", "").Replace("all_access", "");
                speakOutput = $"You have unlocked the {product?.Name}. Here is your {categoryLabel} fact: {FactHelpers.GetRandomFact(acceptedFacts)} {FactHelpers.GetRandomYesNoQuestion()}";
                repromptOutput = FactHelpers.GetRandomYesNoQuestion();
                break;

            case "DECLINED":
                if (request.Name == PaymentType.Buy)
                {
                    speakOutput = $"Thanks for your interest in the {product?.Name}. Would you like another random fact?";
                    repromptOutput = "Would you like another random fact?";
                }
                else
                {
                    var filteredFacts = FactHelpers.GetFilteredFacts(FactData.AllFacts, entitledProducts);
                    speakOutput = $"OK. Here's a random fact: {FactHelpers.GetRandomFact(filteredFacts)} Would you like another random fact?";
                    repromptOutput = "Would you like another random fact?";
                }
                break;

            case "ALREADY_PURCHASED":
                var alreadyFacts = product?.ReferenceName != "all_access"
                    ? FactData.AllFacts.Where(f => f.Type == product?.ReferenceName.Replace("_pack", "")).ToArray()
                    : FactData.AllFacts;
                var alreadyLabel = product?.ReferenceName.Replace("_pack", "").Replace("all_access", "");
                speakOutput = $"Here is your {alreadyLabel} fact: {FactHelpers.GetRandomFact(alreadyFacts)} {FactHelpers.GetRandomYesNoQuestion()}";
                repromptOutput = FactHelpers.GetRandomYesNoQuestion();
                break;

            default:
                _logger.Warning("Unhandled purchaseResult: {PurchaseResult}", request.Payload.PurchaseResult);
                speakOutput = $"Something unexpected happened, but thanks for your interest in the {product?.Name}. Would you like another random fact?";
                repromptOutput = "Would you like another random fact?";
                break;
        }

        return await input.ResponseBuilder
            .Speak(speakOutput)
            .Reprompt(repromptOutput)
            .GetResponse(cancellationToken);
    }
}