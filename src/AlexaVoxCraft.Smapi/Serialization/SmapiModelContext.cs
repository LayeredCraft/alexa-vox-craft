using System.Text.Json.Serialization;
using AlexaVoxCraft.Smapi.Models.InteractionModel;

namespace AlexaVoxCraft.Smapi.Serialization;

// Root types only - see AlexaVoxCraft.Model.Serialization.ModelContext for the same convention.
// SMAPI has no BasePolymorphicConverter/runtime-Type-dispatch converters of its own (confirmed by grep
// during planning), so the only root needed is InteractionModelDefinition - everything else SMAPI-owned
// is reachable transitively from it. SkillInvocationRequest<TRequest>/SkillInvocationResponse<TResponse>
// are open generics closed over consumer-owned types and are intentionally NOT registered here - that is
// exactly the surface AlexaJsonOptions.RegisterTypeInfoResolver exists for the consumer to cover.
[JsonSerializable(typeof(InteractionModelDefinition))]
internal partial class SmapiModelContext : JsonSerializerContext
{
}
