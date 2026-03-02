using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Data;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class YesHandler : IRequestHandler<IntentRequest>
{
    private readonly ILogger<YesHandler> _logger;

    public YesHandler(ILogger<YesHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest
        {
            Intent.Name: BuiltInIntent.Yes or "GetRandomFactIntent"
        });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(YesHandler), nameof(Handle));

        input.AttributesManager.TryGetSessionState<Product[]>("entitledProducts", out var entitledProducts);

        var filteredFacts = FactHelpers.GetFilteredFacts(FactData.AllFacts, entitledProducts);
        var yesNoQuestion = FactHelpers.GetRandomYesNoQuestion();

        return await input.ResponseBuilder
            .Speak($"Here's your random fact: {FactHelpers.GetRandomFact(filteredFacts)} {yesNoQuestion}")
            .Reprompt(yesNoQuestion)
            .GetResponse(cancellationToken);
    }
}