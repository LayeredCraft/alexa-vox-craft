using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Covers the two distinct data shapes legacy exercised for <see cref="AlexaDetail"/>: a recipe
/// (ingredient list) and a TV/media detail screen (buttons + rating).
/// </summary>
public class AlexaDetailTests : TestBase<AlexaDetailTests>
{
    [Fact]
    public async Task AlexaDetail_Serializes_AsRecipe()
    {
        var control = new AlexaDetail
        {
            PrimaryText = "Spaghetti Carbonara",
            SecondaryText = "Serves 4",
            IngredientsText = "Ingredients",
            IngredientsHideDivider = false,
            ImageSource = "https://example.com/carbonara.jpg"
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaDetail_Recipe");
    }

    [Fact]
    public async Task AlexaDetail_Serializes_AsTvDetail()
    {
        var control = new AlexaDetail
        {
            PrimaryText = "The Matrix",
            SecondaryText = "1999 · Sci-Fi",
            BodyText = "A computer hacker learns about the true nature of reality.",
            Button1Text = "Play",
            Button1Theme = "dark",
            Button2Text = "More Info",
            RatingSlotMode = RatingSlotMode.Single,
            RatingNumber = 8.7
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaDetail_TvDetail");
    }
}
