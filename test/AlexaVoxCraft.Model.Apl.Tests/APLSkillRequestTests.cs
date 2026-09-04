using System.Text.Json;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class APLSkillRequestTests : TestBase<APLSkillRequestTests>
{
    [Fact]
    public async Task UserTouchRequest_Deserializes()
    {
        var json = Fx("Requests/UserTouchRequest.json");
        var envelope = JsonSerializer.Deserialize<APLSkillRequest>(json, AlexaJson);

        envelope.Should().NotBeNull();
        envelope.Request.Should().BeOfType<UserEventRequest>();

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task UserEvent_SelectCategory_Deserializes()
    {
        var envelope = Deserialize("Requests/APLUserEvent_SelectCategory.json");

        var userEvent = envelope.Request.Should().BeOfType<UserEventRequest>().Subject;
        userEvent.Arguments.Should().Equal("selectCategory", "general");

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task UserEvent_Answer_Deserializes()
    {
        var envelope = Deserialize("Requests/APLUserEvent_Answer.json");

        var userEvent = envelope.Request.Should().BeOfType<UserEventRequest>().Subject;
        userEvent.Arguments.Should().HaveCount(2);
        userEvent.Arguments![0].Should().BeEquivalentTo("answer");

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task UserEvent_BuyProduct_Deserializes()
    {
        var envelope = Deserialize("Requests/APLUserEvent_BuyProduct.json");

        var userEvent = envelope.Request.Should().BeOfType<UserEventRequest>().Subject;
        userEvent.Token.Should().Be("productDetailToken");

        await TestHelper.VerifyRequestObject(envelope);
    }

    [Fact]
    public async Task UserEvent_ProductDetails_Deserializes()
    {
        var envelope = Deserialize("Requests/APLUserEvent_ProductDetails.json");

        var userEvent = envelope.Request.Should().BeOfType<UserEventRequest>().Subject;
        userEvent.Arguments.Should().HaveCount(2);
        userEvent.Arguments![0].Should().BeEquivalentTo("productDetails");

        await TestHelper.VerifyRequestObject(envelope);
    }

    private static APLSkillRequest Deserialize(string fixturePath)
    {
        var json = Fx(fixturePath);
        var envelope = JsonSerializer.Deserialize<APLSkillRequest>(json, AlexaJson);
        envelope.Should().NotBeNull();
        return envelope!;
    }
}