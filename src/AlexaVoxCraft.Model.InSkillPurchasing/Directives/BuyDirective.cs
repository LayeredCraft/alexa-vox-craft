namespace AlexaVoxCraft.Model.InSkillPurchasing.Directives;

/// <summary>
/// Directive that initiates the Alexa in-skill purchasing Buy flow for the specified product.
/// </summary>
/// <param name="productId">The unique identifier of the in-skill product to purchase.</param>
/// <param name="token">An optional correlation token returned in the resulting <c>Connections.Response</c>.</param>
public class BuyDirective(string? productId = null, string? token = null)
    : PaymentDirective(PaymentType.Buy, token, new PaymentPayload(productId));