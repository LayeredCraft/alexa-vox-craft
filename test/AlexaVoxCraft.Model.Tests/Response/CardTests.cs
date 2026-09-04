using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for <see cref="ICard"/> types. Cards are response objects the skill
/// only ever builds and sends, so they are tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class CardTests() : TestBase<CardTests>
{
    [Fact]
    public async Task SimpleCard_Serializes_WithRealisticContent()
    {
        var card = new SimpleCard
        {
            Title = "Disney Trivia",
            Content = "Question 1. When Disney World opened, who gave the opening speech? 1.  Roy Disney. 2.  Lillian Disney. 3.  The president. 4.  Walt Disney. "
        };

        await TestHelper.VerifySerializedObject(card, AlexaJson, "SimpleCard_RealisticContent");
    }

    [Fact]
    public async Task StandardCard_Serializes()
    {
        var card = new StandardCard
        {
            Title = "Disney Trivia",
            Content = "Question 1. When Disney World opened, who gave the opening speech?",
            Image = new CardImage
            {
                SmallImageUrl = "https://example.com/smallImage.png",
                LargeImageUrl = "https://example.com/largeImage.png"
            }
        };

        await TestHelper.VerifySerializedObject(card, AlexaJson, "StandardCard");
    }

    [Fact]
    public async Task LinkAccountCard_Serializes()
    {
        var card = new LinkAccountCard();

        await TestHelper.VerifySerializedObject(card, AlexaJson, "LinkAccountCard");
    }

    [Fact]
    public async Task AskForPermissionsConsentCard_Serializes()
    {
        var card = new AskForPermissionsConsentCard
        {
            Permissions = ["alexa::household:lists:read", "alexa::household:lists:write"]
        };

        await TestHelper.VerifySerializedObject(card, AlexaJson, "AskForPermissionsConsentCard");
    }
}
