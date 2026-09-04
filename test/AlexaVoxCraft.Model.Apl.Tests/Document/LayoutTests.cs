using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for <see cref="Layout"/> construction, plus the Alexa design-system layout helpers
/// (<see cref="AlexaImage"/>, <see cref="AlexaFooter"/>, <see cref="AlexaHeader"/>). Response-side
/// serialize only (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class LayoutTests() : TestBase<LayoutTests>
{
    [Fact]
    public async Task Layout_Serializes_WithDescriptionParametersAndItems()
    {
        var layout = new Layout(new AlexaHeader(), new AlexaFooter("Hint text"))
        {
            Description = "A basic header with a title and a logo",
            Parameters = [new Parameter("title") { Type = ParameterType.@string }, new Parameter("logoUrl") { Type = ParameterType.@string }]
        };

        await TestHelper.VerifySerializedObject(layout, AlexaJson, "Layout_WithDescriptionParametersAndItems");
    }

    [Fact]
    public async Task AlexaImage_Serializes()
    {
        var image = new AlexaImage
        {
            ImageSource = "https://d2o906d8ln7ui1.cloudfront.net/images/MollyforBT7.png",
            ImageRoundedCorner = true,
            Scale = Scale.BestFit,
            ImageAlignment = AlexaImageAlignment.Center,
            ImageWidth = new AbsoluteDimension(75, "vh"),
            ImageAspectRatio = AlexaImageAspectRatio.Square,
            ImageBlurredBackground = true
        };

        await TestHelper.VerifySerializedObject(image, AlexaJson, "AlexaImage");
    }

    [Fact]
    public async Task AlexaFooter_Serializes()
    {
        var footer = new AlexaFooter("Hint Text");

        await TestHelper.VerifySerializedObject(footer, AlexaJson, "AlexaFooter");
    }

    [Fact]
    public async Task AlexaHeader_Serializes()
    {
        var header = new AlexaHeader
        {
            HeaderTitle = "Header title",
            HeaderSubtitle = "Header subtitle",
            HeaderAttributionImage = "https://d2o906d8ln7ui1.cloudfront.net/images/cheeseskillicon.png",
            HeaderBackgroundColor = "red",
            HeaderBackButton = true,
            HeaderBackButtonAccessibilityLabel = "back",
            HeaderAttributionText = "Attribution",
            HeaderAttributionPrimacy = true,
            HeaderDivider = true,
            LayoutDirection = LayoutDirection.RTL
        };

        await TestHelper.VerifySerializedObject(header, AlexaJson, "AlexaHeader");
    }
}
