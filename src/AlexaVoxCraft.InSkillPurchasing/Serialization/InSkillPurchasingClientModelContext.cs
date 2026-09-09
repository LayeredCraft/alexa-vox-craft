using System.Text.Json.Serialization;
using AlexaVoxCraft.InSkillPurchasing.Models;

namespace AlexaVoxCraft.InSkillPurchasing.Serialization;

// Root types only - see AlexaVoxCraft.Model.Serialization.ModelContext for the same convention.
// Covers AlexaVoxCraft.InSkillPurchasing's own HTTP response models (a distinct package from
// AlexaVoxCraft.Model.InSkillPurchasing, whose directive/response types are covered by
// AlexaVoxCraft.Model.InSkillPurchasing.Serialization.InSkillPurchasingModelContext). No
// BasePolymorphicConverter/runtime-Type-dispatch converters exist in this package (confirmed by grep
// during planning), so only the three HTTP response roots are needed.
[JsonSerializable(typeof(ProductResponse))]
[JsonSerializable(typeof(TransactionResponse))]
[JsonSerializable(typeof(PurchasingEnabled))]
internal partial class InSkillPurchasingClientModelContext : JsonSerializerContext
{
}
