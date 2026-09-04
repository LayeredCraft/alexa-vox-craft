using System.Text.Json;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Component-level coverage for <see cref="Intent"/> in isolation, independent of the request
/// envelope it normally arrives in.
/// </summary>
public sealed class IntentTests() : TestBase<IntentTests>
{
    [Fact]
    public async Task Intent_WithSlotResolution_Deserializes()
    {
        var json = Fx("Components/Intent_WithSlotResolution.json");
        var intent = JsonSerializer.Deserialize<Intent>(json, AlexaJson);

        intent.Should().NotBeNull();
        intent!.Name.Should().Be("CategorySelectionIntent");
        intent.Slots.Should().ContainKey("productCategory");
        intent.Slots["productCategory"].Resolution!.Authorities.Should().ContainSingle()
            .Which.Values.Should().ContainSingle()
            .Which.Value.Name.Should().Be("general");

        await TestHelper.VerifyRequestObject(intent);
    }
}
