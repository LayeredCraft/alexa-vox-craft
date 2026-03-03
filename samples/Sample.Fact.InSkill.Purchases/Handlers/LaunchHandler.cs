using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class LaunchHandler : IRequestHandler<LaunchRequest>
{
    private readonly ILogger<LaunchHandler> _logger;

    public LaunchHandler(ILogger<LaunchHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is LaunchRequest);

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(LaunchHandler), nameof(Handle));

        input.AttributesManager.Session.TryGet<Product[]>("entitledProducts", out var entitledProducts);

        if (entitledProducts is { Length: > 0 })
        {
            var productList = FactHelpers.GetSpeakableListOfProducts(entitledProducts);
            return await input.ResponseBuilder
                .Speak($"Welcome to {Constants.SkillName}. You currently own {productList} products. " +
                       "To hear a random fact, you could say, 'Tell me a fact' or you can ask for a specific " +
                       "category you have purchased, for example, say 'Tell me a science fact'. " +
                       "To know what else you can buy, say, 'What can I buy?'. So, what can I help you with?")
                .Reprompt("I didn't catch that. What can I help you with?")
                .GetResponse(cancellationToken);
        }

        return await input.ResponseBuilder
            .Speak($"Welcome to {Constants.SkillName}. To hear a random fact you can say 'Tell me a fact', " +
                   "or to hear about the premium categories for purchase, say 'What can I buy'. " +
                   "For help, say 'Help me'... So, what can I help you with?")
            .Reprompt("I didn't catch that. What can I help you with?")
            .GetResponse(cancellationToken);
    }
}