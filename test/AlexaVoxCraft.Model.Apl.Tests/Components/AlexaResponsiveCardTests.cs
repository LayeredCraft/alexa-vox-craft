using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Component-level coverage for the remaining "Alexa*" responsive-template card components:
/// AlexaCard, AlexaImageCaption, AlexaPhoto, AlexaTextWrapping.
/// </summary>
public class AlexaResponsiveCardTests : TestBase<AlexaResponsiveCardTests>
{
    [Fact]
    public async Task AlexaCard_Serializes()
    {
        var card = new AlexaCard
        {
            HeaderText = "Disney Trivia",
            PrimaryText = "Question 1",
            SecondaryText = "Multiple choice",
            ImageSource = "https://example.com/card.png",
            CardRoundedCorner = true
        };

        await TestHelper.VerifySerializedObject(card, AlexaJson, "AlexaCard");
    }

    [Fact]
    public async Task AlexaImageCaption_Serializes()
    {
        var caption = new AlexaImageCaption
        {
            PrimaryText = "Caption title"!,
            SecondaryText = "Caption subtitle"!,
            ImageSource = "https://example.com/photo.png"!,
            ImageScrim = true!
        };

        await TestHelper.VerifySerializedObject(caption, AlexaJson, "AlexaImageCaption");
    }

    [Fact]
    public async Task AlexaPhoto_Serializes()
    {
        var photo = new AlexaPhoto
        {
            PrimaryText = "Photo title",
            SecondaryText = "Photo subtitle",
            ImageSource = "https://example.com/photo.png",
            TouchForward = true
        };

        await TestHelper.VerifySerializedObject(photo, AlexaJson, "AlexaPhoto");
    }

    [Fact]
    public async Task AlexaTextWrapping_Serializes()
    {
        var text = new AlexaTextWrapping
        {
            PrimaryText = "Primary"!,
            SecondaryText = "Secondary"!,
            TertiaryText = "Tertiary"!,
            TouchForward = false!
        };

        await TestHelper.VerifySerializedObject(text, AlexaJson, "AlexaTextWrapping");
    }
}
