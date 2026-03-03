namespace AlexaVoxCraft.Model.InSkillPurchasing.Directives;

/// <summary>
/// Directive that initiates the Alexa in-skill purchasing Upsell flow for the specified product.
/// </summary>
/// <param name="productId">The unique identifier of the in-skill product to upsell.</param>
/// <param name="token">An optional correlation token returned in the resulting <c>Connections.Response</c>.</param>
public class UpsellDirective(string? productId = null, string? token = null)
    : PaymentDirective(PaymentType.Upsell, token, new PaymentPayload(productId));