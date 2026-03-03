using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.InSkillPurchasing.RequestHandlers;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Directives;

/// <summary>
/// Base directive for all in-skill purchasing flows (Buy, Cancel, Upsell).
/// Implements <see cref="IEndSessionDirective"/> because the Alexa purchasing flow always ends the current session.
/// </summary>
public class PaymentDirective : ConnectionSendRequest<PaymentPayload>, IEndSessionDirective
{
    /// <summary>
    /// Initializes a new instance of <see cref="PaymentDirective"/> with the specified payment type, correlation token, and payload.
    /// </summary>
    /// <param name="paymentType">The payment type name (e.g., <see cref="PaymentType.Buy"/>).</param>
    /// <param name="correlationToken">An optional token used to correlate the directive with the resulting <c>Connections.Response</c> request.</param>
    /// <param name="payload">The payload containing the target in-skill product.</param>
    public PaymentDirective(string? paymentType = null, string? correlationToken = null, PaymentPayload? payload = null)
    {
        Name = paymentType!;
        Payload = payload!;
        Token = correlationToken;
    }

    /// <summary>Always returns <see langword="true"/>; the purchasing flow requires the session to end.</summary>
    [JsonIgnore] public bool? ShouldEndSession => true;

    /// <summary>Gets or sets the correlation token used to match this directive to its <c>Connections.Response</c>.</summary>
    [JsonPropertyName("token")] public string? Token { get; set; }

    /// <summary>
    /// Registers the <see cref="PaymentDirective"/> type with the directive converter and registers the Buy, Upsell, and Cancel connection request handlers.
    /// Call this once at application startup before processing skill requests.
    /// </summary>
    public static void AddSupport()
    {
        DirectiveConverter.RegisterDirectiveDerivedType<PaymentDirective>(DirectiveType);
        ConnectionSendRequestFactory.Register(new BuyConnectionRequestHandler());
        ConnectionSendRequestFactory.Register(new UpsellConnectionRequestHandler());
        ConnectionSendRequestFactory.Register(new CancelConnectionRequestHandler());
    }
}