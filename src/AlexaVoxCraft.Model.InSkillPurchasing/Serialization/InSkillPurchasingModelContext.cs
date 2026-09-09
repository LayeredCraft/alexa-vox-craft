using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.InSkillPurchasing.Directives;
using AlexaVoxCraft.Model.InSkillPurchasing.Responses;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.InSkillPurchasing.Serialization;

// Root types only - see AlexaVoxCraft.Model.Serialization.ModelContext for the same convention.
// PaymentDirective/BuyDirective/CancelDirective/UpsellDirective are DirectiveConverter dispatch targets
// (PaymentDirective via RegisterDirectiveDerivedType, the other three via the ConnectionSendRequest
// data-driven factory in PaymentDirective.AddSupport()). ConnectionResponseRequest<ConnectionResponsePayload>
// is a ConnectionResponseTypeResolver dispatch target (PaymentConnectionResponseHandler.Create) - it's a
// Model-owned generic closed over an InSkillPurchasing-owned type argument, so the closed instantiation is
// this package's own metadata concern, not Model's.
[JsonSerializable(typeof(PaymentDirective))]
[JsonSerializable(typeof(BuyDirective))]
[JsonSerializable(typeof(CancelDirective))]
[JsonSerializable(typeof(UpsellDirective))]
[JsonSerializable(typeof(ConnectionResponseRequest<ConnectionResponsePayload>))]
internal partial class InSkillPurchasingModelContext : JsonSerializerContext
{
}
