using System.Reflection;
using System.Text;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Lambda.Serialization;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;
using Compono.XunitV3.Aot;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

internal static class EmbeddedFixture
{
    /// <summary>
    /// Loads one of the trusted, realistic Alexa request fixtures embedded from the JIT test suites'
    /// Examples/ directories (see the csproj) - not a new, this-project-only payload. Embedded rather
    /// than read from disk: the published native binary has no dependency on CI's working directory
    /// this way (docs/research/2026-09-11-native-aot-runtime-validation-gaps.md §8).
    /// </summary>
    public static string Load(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Examples.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded fixture resource '{resourceName}' not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}

// Scenario 8 (original console app): Lambda boundary - the real AlexaLambdaSerializer, deserializing
// the same trusted LaunchRequest/IntentRequest fixtures the JIT test suites trust (not a hand-built
// object), proving reuse of a realistic payload through the real production entry point.
public sealed class LambdaBoundaryTests
{
    [Fact]
    public void AlexaLambdaSerializer_Deserialize_RealLaunchRequestFixture()
    {
        var serializer = new AlexaLambdaSerializer(NullLogger<AlexaLambdaSerializer>.Instance, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(EmbeddedFixture.Load("LaunchRequest.json")));

        var launch = serializer.Deserialize<SkillRequest>(stream);

        Assert.IsType<LaunchRequest>(launch?.Request);
    }

    [Fact]
    public void AlexaLambdaSerializer_Deserialize_RealIntentRequestFixture()
    {
        var serializer = new AlexaLambdaSerializer(NullLogger<AlexaLambdaSerializer>.Instance, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(EmbeddedFixture.Load("IntentRequest.json")));

        var intent = serializer.Deserialize<SkillRequest>(stream);

        Assert.IsType<IntentRequest>(intent?.Request);
    }

    [Fact]
    public void AlexaLambdaSerializer_RoundTrips_ConstructedSkillRequest()
    {
        var serializer = new AlexaLambdaSerializer(NullLogger<AlexaLambdaSerializer>.Instance, AlexaJsonOptions.DefaultOptions);
        var payload = new SkillRequest
        {
            Request = new LaunchRequest { Type = "LaunchRequest" },
            Context = new Context
            {
                System = new AlexaSystem { Application = new Application { ApplicationId = "a" } }
            }
        };

        using var outStream = new MemoryStream();
        serializer.Serialize(payload, outStream);
        outStream.Position = 0;
        var roundTripped = serializer.Deserialize<SkillRequest>(outStream);

        Assert.IsType<LaunchRequest>(roundTripped?.Request);
    }
}

// Scenario 9 (original console app): APL Lambda boundary regression proof (Issue #190) - the real
// AlexaLambdaSerializer deserializing the real, trusted APLSkillRequest fixtures the JIT
// AplSkillRequestTests already trust, exercising the exact call trivia-platform's Lambda made when it
// crashed (AlexaLambdaSerializer.Deserialize<APLSkillRequest>). APLSupport.Add() runs once in
// AssemblyModuleInitializer, matching real consumer startup order (before the first request).
// Dispatches through the real ISkillMediator afterward (via the shared MediatorProfile), then
// serializes a response back out through the same serializer.
public sealed class AplLambdaBoundaryTests
{
    [Fact]
    public void AlexaLambdaSerializer_Deserialize_RealUserTouchRequestFixture()
    {
        var serializer = new AlexaLambdaSerializer(NullLogger<AlexaLambdaSerializer>.Instance, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(EmbeddedFixture.Load("UserTouchRequest.json")));

        var userTouch = serializer.Deserialize<APLSkillRequest>(stream);

        Assert.IsType<UserEventRequest>(userTouch?.Request);
    }

    [Fact]
    public void AlexaLambdaSerializer_Deserialize_RealApluserEventAnswerFixture_WithViewportAndVisualContext()
    {
        var serializer = new AlexaLambdaSerializer(NullLogger<AlexaLambdaSerializer>.Instance, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(EmbeddedFixture.Load("APLUserEvent_Answer.json")));

        var userEventAnswer = serializer.Deserialize<APLSkillRequest>(stream);

        Assert.IsType<UserEventRequest>(userEventAnswer?.Request);
        Assert.NotNull(userEventAnswer?.Context.Viewport);
        Assert.NotNull(userEventAnswer?.Context.AplVisualContext);
    }

    [Theory]
    [Compose<MediatorProfile, MediatorSkillId>("amzn1.ask.skill.54b58c306f70c433")]
    public async Task Mediator_DispatchesDeserializedAplSkillRequest_AndSerializesResponse(ISkillMediator mediator)
    {
        var serializer = new AlexaLambdaSerializer(NullLogger<AlexaLambdaSerializer>.Instance, AlexaJsonOptions.DefaultOptions);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(EmbeddedFixture.Load("UserTouchRequest.json")));
        var userTouch = serializer.Deserialize<APLSkillRequest>(stream);
        Assert.NotNull(userTouch);

        AmbientRequest.Current = userTouch;

        var response = await mediator.Send(userTouch, CancellationToken.None);

        var ssml = Assert.IsType<SsmlOutputSpeech>(response.Response?.OutputSpeech);
        Assert.Contains("hello from the native AOT test project's APL handler", ssml.Ssml);

        using var responseStream = new MemoryStream();
        serializer.Serialize(response, responseStream);
        Assert.True(responseStream.Length > 0);
    }
}
