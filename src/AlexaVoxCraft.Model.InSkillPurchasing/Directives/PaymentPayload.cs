using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Directives;

/// <summary>
/// Represents the payload sent with an in-skill purchasing directive, containing the target product and an optional upsell message.
/// </summary>
public class PaymentPayload
{
    /// <summary>
    /// Initializes a new instance of <see cref="PaymentPayload"/> with the specified product ID and optional upsell message.
    /// </summary>
    /// <param name="productId">The unique identifier of the in-skill product.</param>
    /// <param name="upsellMessage">An optional message displayed to the user during an upsell flow.</param>
    public PaymentPayload(string? productId, string? upsellMessage = null)
    {
        InSkillProduct = new PaymentPayloadProduct(productId);
        UpsellMessage = upsellMessage;
    }

    /// <summary>Gets or sets the in-skill product being purchased, canceled, or upsold.</summary>
    [JsonPropertyName("InSkillProduct")]
    public PaymentPayloadProduct? InSkillProduct { get; set; }

    /// <summary>Gets or sets the message presented to the user during an upsell flow. Omitted from serialization when <see langword="null"/>.</summary>
    [JsonPropertyName("upsellMessage"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UpsellMessage { get; set; }
}