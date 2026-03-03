using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.InSkillPurchasing.Directives;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Data;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class GetCategoryFactHandler : IRequestHandler<IntentRequest>
{
    private readonly IInSkillPurchasingClient _ispClient;
    private readonly ILogger<GetCategoryFactHandler> _logger;

    public GetCategoryFactHandler(IInSkillPurchasingClient ispClient, ILogger<GetCategoryFactHandler> logger)
    {
        _ispClient = ispClient ?? throw new ArgumentNullException(nameof(ispClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: "GetCategoryFactIntent" });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(GetCategoryFactHandler), nameof(Handle));

        var request = (IntentRequest)input.RequestEnvelope.Request;
        var factCategory = FactHelpers.GetResolvedSlotValue(request, "factCategory");

        if (factCategory is null)
        {
            var spokenValue = FactHelpers.GetSpokenSlotValue(request, "factCategory");
            var speakPrefix = spokenValue is not null ? $"I heard you say {spokenValue}. " : string.Empty;
            return await input.ResponseBuilder
                .Speak($"{speakPrefix}I don't have facts for that category. You can ask for science, space, or history facts. Which one would you like?")
                .Reprompt("Which fact category would you like? I have science, space, or history.")
                .GetResponse(cancellationToken);
        }

        switch (factCategory)
        {
            case "free":
                {
                    var facts = FactData.AllFacts.Where(f => f.Type == "free").ToArray();
                    return await input.ResponseBuilder
                        .Speak($"Here's your free fact: {FactHelpers.GetRandomFact(facts)} {FactHelpers.GetRandomYesNoQuestion()}")
                        .Reprompt(FactHelpers.GetRandomYesNoQuestion())
                        .GetResponse(cancellationToken);
                }

            case "random":
            case "all_access":
                {
                    input.AttributesManager.Session.TryGet<Product[]>("entitledProducts", out var entitledProducts);
                    var filteredFacts = FactHelpers.GetFilteredFacts(FactData.AllFacts, entitledProducts);
                    return await input.ResponseBuilder
                        .Speak($"Here's your random fact: {FactHelpers.GetRandomFact(filteredFacts)} {FactHelpers.GetRandomYesNoQuestion()}")
                        .Reprompt(FactHelpers.GetRandomYesNoQuestion())
                        .GetResponse(cancellationToken);
                }

            default:
                {
                    var categoryFacts = FactData.AllFacts.Where(f => f.Type == factCategory).ToArray();
                    var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
                    var products = result?.Products ?? [];

                    var subscription = products.FirstOrDefault(p => p.ReferenceName == "all_access");
                    var categoryProduct = products.FirstOrDefault(p => p.ReferenceName == $"{factCategory}_pack");

                    var hasAccess = subscription?.Entitled == Entitled.ENTITLED ||
                                    categoryProduct?.Entitled == Entitled.ENTITLED;

                    if (hasAccess)
                    {
                        return await input.ResponseBuilder
                            .Speak($"Here's your {factCategory} fact: {FactHelpers.GetRandomFact(categoryFacts)} {FactHelpers.GetRandomYesNoQuestion()}")
                            .Reprompt(FactHelpers.GetRandomYesNoQuestion())
                            .GetResponse(cancellationToken);
                    }

                    if (categoryProduct is not null)
                    {
                        var upsellMessage = $"You don't currently own the {factCategory} pack. {categoryProduct.Summary} Want to learn more?";
                        return await input.ResponseBuilder
                            .AddDirective(new UpsellDirective(categoryProduct.ProductId, "correlationToken")
                            {
                                Payload = new(categoryProduct.ProductId, upsellMessage)
                            })
                            .GetResponse(cancellationToken);
                    }

                    _logger.Warning("Category {Category} had no matching ISP product", factCategory);
                    return await input.ResponseBuilder
                        .Speak($"I'm having trouble accessing the {factCategory} facts right now. Try a different category for now. {FactHelpers.GetRandomYesNoQuestion()}")
                        .Reprompt(FactHelpers.GetRandomYesNoQuestion())
                        .GetResponse(cancellationToken);
                }
        }
    }
}