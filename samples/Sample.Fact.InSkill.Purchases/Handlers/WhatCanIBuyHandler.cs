using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class WhatCanIBuyHandler : IRequestHandler<IntentRequest>
{
    private readonly IInSkillPurchasingClient _ispClient;
    private readonly ILogger<WhatCanIBuyHandler> _logger;

    public WhatCanIBuyHandler(IInSkillPurchasingClient ispClient, ILogger<WhatCanIBuyHandler> logger)
    {
        _ispClient = ispClient ?? throw new ArgumentNullException(nameof(ispClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: "WhatCanIBuyIntent" });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(WhatCanIBuyHandler), nameof(Handle));

        var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
        var purchasableProducts = result?.Products
            .Where(p => p.Entitled == Entitled.NOT_ENTITLED && p.Purchasable == Purchasable.PURCHASABLE)
            .ToArray() ?? [];

        if (purchasableProducts.Length > 0)
        {
            return await input.ResponseBuilder
                .Speak($"Products available for purchase at this time are {FactHelpers.GetSpeakableListOfProducts(purchasableProducts)}. " +
                       "To learn more about a product, say 'Tell me more about' followed by the product name. " +
                       "If you are ready to buy say 'Buy' followed by the product name. So what can I help you with?")
                .Reprompt("I didn't catch that. What can I help you with?")
                .GetResponse(cancellationToken);
        }

        _logger.Warning("The product list came back as empty. This could be due to no ISPs being created and linked to the skill.");

        return await input.ResponseBuilder
            .Speak("I've checked high and low, however I can't find any products to offer to you right now. Sorry about that. " +
                   "Would you like a random fact now instead?")
            .Reprompt("I didn't catch that. What can I help you with?")
            .GetResponse(cancellationToken);
    }
}