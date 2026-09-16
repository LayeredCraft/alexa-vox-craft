using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Serialization;
using AlexaVoxCraft.Smapi.Models.Invocation;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

public sealed class GameState
{
    public int Score { get; set; }
    public string? Level { get; set; }
}

/// <summary>
/// Deliberately never added to <see cref="ValidationAppContext"/>'s <c>[JsonSerializable]</c> list -
/// the one type in this project that must always fail to serialize under reflection-disabled AOT,
/// proving the failure is clear and immediate rather than a silent fallback.
/// </summary>
public sealed class UnregisteredValidationPoco
{
    public string? Value { get; set; }
}

[JsonSerializable(typeof(GameState))]
[JsonSerializable(typeof(SkillInvocationResponse<GameState>))]
[JsonSerializable(typeof(SkillInvocationRequest<GameState>))]
[JsonSerializable(typeof(SkillInvocationBody<GameState>))]
[JsonSerializable(typeof(SkillInvocationResult<GameState>))]
[JsonSerializable(typeof(SkillExecutionInfo<GameState>))]
[JsonSerializable(typeof(InvocationResponseInfo<GameState>))]
internal partial class ValidationAppContext : JsonSerializerContext;

public sealed class ConsumerMetadataTests
{
    // Scenario 3 (original console app): consumer-owned metadata - a project-owned POCO with its own
    // generated context, registered via RegisterTypeInfoResolver (done once, in
    // AssemblyModuleInitializer), used through JsonAttributeBag. Also proves library metadata still
    // wins for library types after the consumer context registers.
    [Fact]
    public void ConsumerPoco_RoundTrips_ViaJsonAttributeBag()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
        bag.Set("state", new GameState { Score = 42, Level = "final" });

        var readBack = bag.Get<GameState>("state");

        Assert.NotNull(readBack);
        Assert.Equal(42, readBack.Score);
        Assert.Equal("final", readBack.Level);
    }

    [Fact]
    public void LibraryType_StillResolves_AfterConsumerRegistration()
    {
        var stillWorks = JsonSerializer.Deserialize(
            """{"type":"LaunchRequest"}""", AlexaJsonTypeInfo.For<LaunchRequest>());

        Assert.NotNull(stillWorks);
    }

    // Scenario 4: missing consumer metadata fails clearly and immediately (required negative scenario).
    // Asserted at AlexaJsonTypeInfo.For<T>()'s own GetTypeInfo(Type) call (confirmed by direct probe to
    // throw the identical NotSupportedException an unregistered type would also produce from
    // JsonSerializer.Serialize<T>(value, options) directly) - failing at metadata resolution, before
    // any attempt to serialize, is if anything a more precise proof of "fails clearly and immediately"
    // than the original call shape.
    [Fact]
    public void UnregisteredType_FailsClearly_UnderReflectionDisabledAot()
    {
        Assert.Throws<NotSupportedException>(() => AlexaJsonTypeInfo.For<UnregisteredValidationPoco>());
    }
}
