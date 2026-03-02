using AlexaVoxCraft.InSkillPurchasing.Clients;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;
using LayeredCraft.StructuredLogging;
using Microsoft.Extensions.Logging;

namespace Sample.Fact.InSkill.Purchases.Interceptors;

public class EntitledProductsInterceptor : IRequestInterceptor
{
    private readonly IInSkillPurchasingClient _ispClient;
    private readonly ILogger<EntitledProductsInterceptor> _logger;

    public EntitledProductsInterceptor(IInSkillPurchasingClient ispClient, ILogger<EntitledProductsInterceptor> logger)
    {
        _ispClient = ispClient ?? throw new ArgumentNullException(nameof(ispClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        if (input.RequestEnvelope.Session?.New != true)
            return;

        try
        {
            var result = await _ispClient.GetProductsAsync(cancellationToken: cancellationToken);
            var entitledProducts = result?.Products
                .Where(p => p.Entitled == Entitled.ENTITLED)
                .ToArray() ?? [];

            _logger.Information("Currently entitled products: {Products}", entitledProducts.Select(p => p.Name));

            input.AttributesManager.SetSessionState("entitledProducts", entitledProducts);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error calling InSkillProducts API");
        }
    }
}