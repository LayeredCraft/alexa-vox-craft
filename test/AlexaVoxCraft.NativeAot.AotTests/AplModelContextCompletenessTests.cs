using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Issue #198: adding the 4 types that broke a real consumer wasn't enough - the actual defect is that
// AplModelContext's coverage strategy has no path that reaches types like these at all, so any future
// APLValue<T>/APLValueCollection<T> instantiation with a new T is silently the same gap again the moment
// it ships, until some consumer's production traffic happens to exercise it.
//
// APLValueConverterFactory._valueConverters and APLValueCollectionConverterFactory._itemConverters are
// themselves already the library's own closed, intentionally-maintained declaration of "every T this
// library ships APLValue<T>/APLValueCollection<T> support for" (per their own header comments - "every
// APLValue<T> instantiation this library ships must be registered"). That makes them the right source of
// truth for this test to walk, rather than reflecting over every public type in the assembly: it defines
// the *intended* serialization surface by actual package semantics (per T, does this library claim to
// support serializing it wrapped in APLValue<T>/APLValueCollection<T>), not "is it public".
//
// This is a completeness test, not a re-statement of AplModelContextCoverageTests' specific repro cases:
// it will fail for *any* T added to either dispatch table in the future without a matching AplModelContext
// root, not just the 4 types already found and fixed.
public sealed class AplModelContextCompletenessTests
{
    // Types System.Text.Json's source-generated infrastructure resolves without an explicit
    // [JsonSerializable] root anywhere - confirmed empirically against AplModelContext.Default during
    // this investigation. Every APLValue<T>/APLValueCollection<T> entry that ISN'T one of these must have
    // its own explicit root, because it's reached only via the object-erasure path described in
    // AplModelContextCoverageTests, which resolves purely by runtime type.
    private static readonly HashSet<Type> KnownTypes =
    [
        typeof(string), typeof(bool), typeof(int), typeof(double), typeof(object), typeof(Uri)
    ];

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(APLValueConverterFactory))]
    public static IEnumerable<object[]> ValueConverterTargetTypes() =>
        GetDictionaryKeys(typeof(APLValueConverterFactory), "_valueConverters")
            .Select(NormalizeNullable)
            .Where(t => !KnownTypes.Contains(t))
            .Distinct()
            .Select(t => new object[] { t });

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(APLValueCollectionConverterFactory))]
    public static IEnumerable<object[]> CollectionConverterTargetTypes() =>
        GetDictionaryKeys(typeof(APLValueCollectionConverterFactory), "_itemConverters")
            .Select(NormalizeNullable)
            .Where(t => !KnownTypes.Contains(t) && !t.IsInterface)
            .Distinct()
            .Select(t => new object[] { t });

    [Theory]
    [MemberData(nameof(ValueConverterTargetTypes))]
    public void EveryAPLValueTarget_HasAplModelContextCoverage(Type valueType)
    {
        AssertResolvable(valueType, nameof(APLValueConverterFactory));
    }

    [Theory]
    [MemberData(nameof(CollectionConverterTargetTypes))]
    public void EveryAPLValueCollectionTarget_HasAplModelContextCoverage(Type itemType)
    {
        // Concrete component/command classes here are covered via polymorphic dispatch (their own
        // RegisterTypeInfo<T>() call or a BasePolymorphicConverter<T> DerivedTypes entry), not
        // necessarily a direct root for the interface/base key itself - only plain classes/enums need a
        // direct root. Skip types this library resolves polymorphically (has a converter attribute
        // pointing at a *ByBase/ConverterFactory type) - those are validated by
        // AplTests/CoreJsonTests round-tripping real component graphs instead.
        if (IsPolymorphicallyDispatched(itemType))
        {
            return;
        }

        AssertResolvable(itemType, nameof(APLValueCollectionConverterFactory));
    }

    private static void AssertResolvable(Type type, string sourceTable)
    {
        var typeInfo = AlexaJsonOptions.DefaultOptions.GetTypeInfo(type);

        Assert.True(
            typeInfo is not null,
            $"{type} is a target type in {sourceTable}'s closed dispatch table but has no " +
            "JsonTypeInfo resolvable through AlexaJsonOptions.DefaultOptions. Add " +
            $"[JsonSerializable(typeof({type.Name}))] to AplModelContext.");
    }

    private static bool IsPolymorphicallyDispatched(Type type) =>
        type.GetCustomAttributes(inherit: false)
            .Any(a => a.GetType().Name.Contains("JsonConverter", StringComparison.Ordinal));

    private static Type NormalizeNullable(Type type) => Nullable.GetUnderlyingType(type) ?? type;

    // Test-time-only reflection over two specific, hand-maintained private fields to build this test's
    // input set. Under real Native AOT publish (confirmed by running the published native binary, not
    // just `dotnet run`) the linker trims/renames these fields by default since nothing else in the
    // reachability graph references them by name - the DynamicDependency attributes on the two callers
    // below preserve them explicitly, which is the correct fix (not a suppression): this reflection is
    // intentional and its two targets are fixed, so telling the trimmer to keep them is accurate, not a
    // workaround.
    [UnconditionalSuppressMessage(
        "Trimming", "IL2070", Justification = "Test-only reflection over a fixed, known field name.")]
    private static IEnumerable<Type> GetDictionaryKeys(Type declaringType, string fieldName)
    {
        var field = declaringType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException(
                $"{declaringType}.{fieldName} not found - this test's reflection target has moved; " +
                "update GetDictionaryKeys' field name to match.");

        var dictionary = (IDictionary)field.GetValue(null)!;

        return dictionary.Keys.Cast<Type>();
    }
}
