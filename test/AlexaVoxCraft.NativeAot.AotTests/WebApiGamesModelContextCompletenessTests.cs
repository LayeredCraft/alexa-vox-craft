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

    private static IEnumerable<Type> DirectiveDispatchTargets() =>
        GetFieldValue(typeof(DirectiveConverter), "_directiveDerivedTypes")
            .OfType<Type>()
            .Where(t => t.Assembly == typeof(StartDirective).Assembly);

    private static IEnumerable<Type> RequestDispatchTargets() =>
        GetFieldValue(typeof(RequestConverter), "RequestTypeResolvers")
            .OfType<IRequestTypeResolver>()
            .Where(resolver => resolver.CanResolve(MessageRequest.RequestType))
            .Select(resolver => resolver.Resolve(MessageRequest.RequestType))
            .OfType<Type>()
            .Where(t => t.Assembly == typeof(MessageRequest).Assembly);

    // Test-time-only reflection over two specific, hand-maintained private fields to build this test's
    // input set. Rather than suppress the trimmer's IL2070 finding, the declaringType parameter carries
    // the same NonPublicFields annotation Type.GetField requires, and every caller passes a typeof(...)
    // constant - so the analyzer is satisfied and the trimmer preserves the fields the reflection needs.
    private static IEnumerable<object?> GetFieldValue(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicFields)] Type declaringType,
        string fieldName)
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
