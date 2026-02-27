using AlexaVoxCraft.Model.InSkillPurchasing.Directives;
using AlexaVoxCraft.Model.InSkillPurchasing.Responses;

namespace AlexaVoxCraft.Model.InSkillPurchasing;

/// <summary>
/// Entry point for registering all in-skill purchasing support with the AlexaVoxCraft serialization infrastructure.
/// </summary>
public class InSkillPurchasingSupport
{
    /// <summary>
    /// Registers payment directives and connection response handlers required for in-skill purchasing.
    /// Call this once at application startup before processing skill requests.
    /// </summary>
    public static void Add()
    {
        PaymentDirective.AddSupport();
        ConnectionResponseHandler.AddSupport();
    }
}