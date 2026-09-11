using AlexaVoxCraft.Model.Apl.Serialization;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests.Serialization;

/// <summary>
/// Closes the completeness-test gap that let Issue #190 (docs/research/2026-09-11-native-aot-runtime-
/// validation-gaps.md §4; docs/plans/0004-native-aot-runtime-fixes-and-validation.md Task Group 4)
/// escape: <see cref="AplModelContextCompletenessTests"/> only walks types reachable through
/// <c>BasePolymorphicConverter&lt;T&gt;.DerivedTypes</c> - i.e. types dispatched to <i>polymorphically</i>
/// from an already-known parent. A type a consumer passes directly as <c>T</c> to
/// <c>JsonSerializer.Deserialize&lt;T&gt;</c>/<c>AlexaLambdaSerializer.Deserialize&lt;T&gt;</c> (a "root
/// type") is never a polymorphic dispatch target, so that walk is structurally blind to a missing root
/// registration - which is exactly how <c>APLSkillRequest</c> shipped without metadata in either
/// <see cref="ModelContext"/> or <see cref="AplModelContext"/>.
///
/// This test enumerates, deliberately by hand (not via a reflection scan of "every public type in the
/// assembly" - the intent is to prove the specific documented entry points stay covered, not to police
/// every type in the library), the small set of types a consumer is documented/expected to pass directly
/// as a serializer root, and asserts each has a <c>JsonTypeInfo</c> in its owning context's
/// <c>.Default</c> resolver.
/// </summary>
public class RootTypeCompletenessTests
{
    public static IEnumerable<object[]> KnownRootTypes()
    {
        // SkillRequest: the base request envelope, owned by AlexaVoxCraft.Model - any non-APL consumer
        // (e.g. AlexaSkillFunction<SkillRequest, SkillResponse>) deserializes directly into this type.
        yield return [typeof(SkillRequest), "ModelContext", (Func<Type, bool>)(t => ModelContext.Default.GetTypeInfo(t) is not null)];

        // APLSkillRequest: the APL-capable request envelope, owned by AlexaVoxCraft.Model.Apl - any
        // APL-capable consumer (e.g. AlexaSkillFunction<APLSkillRequest, SkillResponse>, per
        // samples/Sample.Apl.Function) deserializes directly into this type. This is the exact type
        // Issue #190 was missing.
        yield return [typeof(APLSkillRequest), "AplModelContext", (Func<Type, bool>)(t => AplModelContext.Default.GetTypeInfo(t) is not null)];
    }

    [Theory]
    [MemberData(nameof(KnownRootTypes))]
    public void KnownRootType_HasJsonTypeInfo_InItsOwningContext(Type rootType, string owningContextName, Func<Type, bool> hasMetadata)
    {
        hasMetadata(rootType).Should().BeTrue(
            $"{rootType.FullName} is a documented root type consumers deserialize directly - " +
            $"it must have JsonTypeInfo in {owningContextName} or it fails only under Native AOT (Issue #190), " +
            "invisible under JIT because of AlexaJsonOptions' reflection fallback.");
    }
}
