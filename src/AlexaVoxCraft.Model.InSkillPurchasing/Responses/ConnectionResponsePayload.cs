using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Responses;

/// <summary>
/// Payload returned in a <c>Connections.Response</c> request after an in-skill purchasing flow completes.
/// </summary>
public class ConnectionResponsePayload
{
    /// <summary>Gets or sets the result of the purchase transaction (e.g., <c>ACCEPTED</c>, <c>DECLINED</c>, <c>ALREADY_PURCHASED</c>, <c>ERROR</c>).</summary>
    [JsonPropertyName("purchaseResult")]
    public string PurchaseResult { get; set; }

    /// <summary>Gets or sets the unique identifier of the in-skill product involved in the transaction.</summary>
    [JsonPropertyName("productId")]
    public string ProductId { get; set; }

    /// <summary>Gets or sets an optional message providing additional context about the transaction result.</summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}