using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Directives;

/// <summary>
/// Represents the in-skill product reference within a payment payload, identified by its product ID.
/// </summary>
public class PaymentPayloadProduct
{
    /// <summary>
    /// Initializes a new instance of <see cref="PaymentPayloadProduct"/> with the specified product ID.
    /// </summary>
    /// <param name="productId">The unique identifier of the in-skill product.</param>
    public PaymentPayloadProduct(string? productId)
    {
        ProductId = productId;
    }

    /// <summary>Gets or sets the unique identifier of the in-skill product.</summary>
    [JsonPropertyName("productId")]
    public string? ProductId { get; set; }
}