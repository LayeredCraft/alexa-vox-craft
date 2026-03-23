namespace AlexaVoxCraft.Model.InSkillPurchasing.Directives;

/// <summary>
/// Directive that initiates the Alexa in-skill purchasing Upsell flow for the specified product.
/// </summary>
/// <param name="productId">The unique identifier of the in-skill product to upsell.</param>
/// <param name="upsellMessage">The message presented to the user during the upsell flow.</param>
/// <param name="token">A correlation token returned in the resulting <c>Connections.Response</c>.</param>
public class UpsellDirective(string productId, string upsellMessage, string token)
    : PaymentDirective(PaymentType.Upsell, token, new PaymentPayload(productId, upsellMessage));