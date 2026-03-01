using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.InSkillPurchasing.Directives;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class CancelSubscriptionHandler : IRequestHandler<IntentRequest>
{
    private readonly IInSkillPurchasingClient _ispClient;
    private readonly ILogger<CancelSubscriptionHandler> _logger;

    public CancelSubscriptionHandler(IInSkillPurchasingClient ispClient, ILogger<CancelSubscriptionHandler> logger)
    {
        _ispClient = ispClient ?? throw new ArgumentNullException(nameof(ispClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: "CancelSubscriptionIntent" });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(CancelSubscriptionHandler), nameof(Handle));

        var request = (IntentRequest)input.RequestEnvelope.Request;
        var productCategory = FactHelpers.GetResolvedSlotValue(request, "productCategory");

        var referenceName = productCategory is null ? "all_access"
            : productCategory != "all_access" ? productCategory + "_pack"
            : productCategory;

        var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
        var product = result?.Products.FirstOrDefault(p => p.ReferenceName == referenceName);

        if (product is not null)
        {
            return await input.ResponseBuilder
                .AddDirective(new CancelDirective(product.ProductId, "correlationToken"))
                .GetResponse(cancellationToken);
        }

        _logger.Warning("Requested product {ReferenceName} could not be found", referenceName);

        return await input.ResponseBuilder
            .Speak("I don't think we have a product by that name. Can you try again?")
            .Reprompt("I didn't catch that. Can you try again?")
            .GetResponse(cancellationToken);
    }
}