using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Coverage for the <see cref="RequestConverter.RegisterRequestTypeResolver{TResolver}"/>
/// extensibility hook (a consumer registering a wholly new top-level request type discriminator),
/// and the epoch-timestamp <see cref="LaunchRequest"/> parsing variant.
/// </summary>
public sealed class CustomRequestTypeTests() : TestBase<CustomRequestTypeTests>
{
    [Fact]
    public void CustomRequestType_Deserializes_ViaRegisteredResolver()
    {
        RequestConverter.RegisterRequestTypeResolver<CustomIntentRequestTypeResolver>();

        var json = Fx("Requests/LaunchRequest_CustomType.json");
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        var request = envelope!.Request.Should().BeOfType<CustomIntentRequest>().Subject;
        request.TestProperty.Should().BeTrue();
    }

    [Fact]
    public async Task LaunchRequest_WithEpochTimestamp_Deserializes()
    {
        var json = Fx("Requests/LaunchRequest_EpochTimestamp.json");
        var envelope = JsonSerializer.Deserialize<SkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope!.Request.Should().BeOfType<LaunchRequest>();

        await TestHelper.VerifyRequestObject(envelope);
    }
}

/// <summary>A custom, non-built-in request type used to exercise the resolver-registration hook.</summary>
public class CustomIntentRequest : AlexaVoxCraft.Model.Request.Type.Request
{
    [System.Text.Json.Serialization.JsonPropertyName("testProperty")]
    public bool TestProperty { get; set; }
}

public class CustomIntentRequestTypeResolver : IRequestTypeResolver
{
    public bool CanResolve(string requestType) => requestType == "AlexaNet.CustomIntent";

    public Type Resolve(string requestType) => typeof(CustomIntentRequest);
}
