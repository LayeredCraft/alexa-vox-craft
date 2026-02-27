namespace AlexaVoxCraft.Model.InSkillPurchasing;

/// <summary>
/// Defines the payment type names used as directive identifiers for in-skill purchasing flows.
/// </summary>
public class PaymentType
{
    /// <summary>The payment type for initiating a purchase of an in-skill product.</summary>
    public const string Buy = nameof(Buy);

    /// <summary>The payment type for canceling an active in-skill subscription or purchase.</summary>
    public const string Cancel = nameof(Cancel);

    /// <summary>The payment type for presenting an upsell offer for an in-skill product.</summary>
    public const string Upsell = nameof(Upsell);
}