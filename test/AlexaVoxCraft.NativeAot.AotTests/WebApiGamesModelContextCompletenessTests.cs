using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Serialization;
using AlexaVoxCraft.Model.WebApiGames;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Issue #213: every Web API for Games type the runtime dispatches through the polymorphic converters -
// the DirectiveConverter dispatch targets and the RequestConverter resolver target - must have an
// explicit root (or generated nested metadata) in WebApiGamesModelContext, because both converters erase
// to the value's runtime type before their inner JsonSerializer call. Like AplModelContextCompletenessTests,
// the runtime-dispatch roots are derived from the library's own closed dispatch tables rather than a
// hand-copied list, so a future Web API for Games directive/request type added without a matching root
// fails here instead of shipping. The three nested types (StartRequest, StartConfiguration,
// HTMLTransformer) are reached only as properties of a dispatched root, so they carry no table entry and
// are listed explicitly.
public sealed class WebApiGamesModelContextCompletenessTests
{
    private static readonly Type[] NestedRuntimeTypes =
    [
        typeof(StartRequest),
        typeof(StartConfiguration),
        typeof(HTMLTransformer)
    ];

    public static IEnumerable<object[]> RuntimeDispatchedWebApiGamesTypes() =>
        DirectiveDispatchTargets()
            .Concat(RequestDispatchTargets())
            .Concat(NestedRuntimeTypes)
            .Distinct()
            .Select(type => new object[] { type });

    [Theory]
    [MemberData(nameof(RuntimeDispatchedWebApiGamesTypes))]
    public void EveryRuntimeDispatchedWebApiGamesType_ResolvesThroughDefaultOptions(Type type)
    {
        var typeInfo = AlexaJsonOptions.DefaultOptions.GetTypeInfo(type);

        Assert.True(
            typeInfo is not null,
            $"{type} is dispatched at runtime through a Web API for Games converter but has no " +
            "JsonTypeInfo resolvable through AlexaJsonOptions.DefaultOptions. Add " +
            $"[JsonSerializable(typeof({type.Name}))] (or make it reachable from an existing root) in " +
            "WebApiGamesModelContext.");
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(DirectiveConverter))]
    private static IEnumerable<Type> DirectiveDispatchTargets() =>
        GetFieldValue(typeof(DirectiveConverter), "_directiveDerivedTypes")
            .OfType<Type>()
            .Where(t => t.Assembly == typeof(StartDirective).Assembly);

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, typeof(RequestConverter))]
    private static IEnumerable<Type> RequestDispatchTargets() =>
        GetFieldValue(typeof(RequestConverter), "RequestTypeResolvers")
            .OfType<IRequestTypeResolver>()
            .Where(resolver => resolver.CanResolve(MessageRequest.RequestType))
            .Select(resolver => resolver.Resolve(MessageRequest.RequestType))
            .OfType<Type>()
            .Where(t => t.Assembly == typeof(MessageRequest).Assembly);

    // Test-time-only reflection over two specific, hand-maintained private fields to build this test's
    // input set (mirrors AplModelContextCompletenessTests.GetDictionaryKeys). Under real Native AOT
    // publish the trimmer would otherwise remove these fields since nothing else references them by name;
    // the DynamicDependency attributes on the two callers above preserve them explicitly - accurate, not
    // a workaround, because this reflection is intentional and its two targets are fixed.
    [UnconditionalSuppressMessage(
        "Trimming", "IL2070", Justification = "Test-only reflection over a fixed, known field name.")]
    private static IEnumerable<object?> GetFieldValue(Type declaringType, string fieldName)
    {
        var field = declaringType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException(
                $"{declaringType}.{fieldName} not found - this test's reflection target has moved; " +
                "update GetFieldValue's field name to match.");

        return field.GetValue(null) switch
        {
            IDictionary dictionary => dictionary.Values.Cast<object?>(),
            IEnumerable enumerable => enumerable.Cast<object?>(),
            _ => throw new InvalidOperationException($"{declaringType}.{fieldName} is not enumerable.")
        };
    }
}
