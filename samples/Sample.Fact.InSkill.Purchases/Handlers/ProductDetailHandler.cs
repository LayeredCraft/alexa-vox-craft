using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;
using Sample.Fact.InSkill.Purchases.Helpers;

namespace Sample.Fact.InSkill.Purchases.Handlers;

public class ProductDetailHandler : IRequestHandler<IntentRequest>
{
    private readonly IInSkillPurchasingClient _ispClient;
    private readonly ILogger<ProductDetailHandler> _logger;

    public ProductDetailHandler(IInSkillPurchasingClient ispClient, ILogger<ProductDetailHandler> logger)
    {
        _ispClient = ispClient ?? throw new ArgumentNullException(nameof(ispClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> CanHandle(IHandlerInput input, CancellationToken cancellationToken = default) =>
        Task.FromResult(input.RequestEnvelope.Request is IntentRequest { Intent.Name: "ProductDetailIntent" });

    public async Task<SkillResponse> Handle(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        _logger.Information("IN: {Handler}.{Method}", nameof(ProductDetailHandler), nameof(Handle));

        var request = (IntentRequest)input.RequestEnvelope.Request;
        var spokenCategory = FactHelpers.GetSpokenSlotValue(request, "productCategory");

        if (spokenCategory is null)
        {
            return await input.ResponseBuilder
                .AddDelegateDirective()
                .GetResponse(cancellationToken);
        }

        var productCategory = FactHelpers.GetResolvedSlotValue(request, "productCategory");

        if (productCategory is null)
        {
            return await input.ResponseBuilder
                .Speak("I don't think we have a product by that name. Can you try again?")
                .Reprompt("I didn't catch that. Can you try again?")
                .GetResponse(cancellationToken);
        }

        var referenceName = productCategory != "all_access" ? productCategory + "_pack" : productCategory;

        var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
        var product = result?.Products.FirstOrDefault(p => p.ReferenceName == referenceName);

        if (product is not null)
        {
            return await input.ResponseBuilder
                .Speak($"{product.Summary}. To buy it, say Buy {product.Name}.")
                .Reprompt($"I didn't catch that. To buy {product.Name}, say Buy {product.Name}.")
                .GetResponse(cancellationToken);
        }

        _logger.Warning("Requested product {ReferenceName} could not be found", referenceName);

        return await input.ResponseBuilder
            .Speak("I can't find a product by that name. Can you try again?")
            .Reprompt("I didn't catch that. Can you try again?")
            .GetResponse(cancellationToken);
    }
}