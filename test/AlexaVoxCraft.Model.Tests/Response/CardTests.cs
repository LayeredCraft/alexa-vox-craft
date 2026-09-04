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
}
