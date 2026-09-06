using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for the legacy (pre-APL) Display interface's template building blocks.
/// This interface is unused by any skill this session has real captures for, so all data is
/// synthetic. Templates are response objects the skill only ever builds and sends, so they are
/// tested via serialize, not deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class DisplayTemplateTests() : TestBase<DisplayTemplateTests>
{
    private const string ImageSourceUrl = "https://example.com/resources/card-images/mount-saint-helen-small.png";
    private const string ImageDescription = "Mount St. Helens landscape";

    [Fact]
    public async Task TemplateImage_Serializes_Basic()
    {
        var image = new TemplateImage
        {
            ContentDescription = ImageDescription,
            Sources = [new ImageSource { Url = ImageSourceUrl }]
        };

        await TestHelper.VerifySerializedObject(image, AlexaJson, "TemplateImage_Basic");
    }

    [Fact]
    public async Task TemplateImage_Serializes_WithSizeAndDimensions()
    {
        var image = new TemplateImage
        {
            ContentDescription = ImageDescription,
            Sources = [new ImageSource { Url = ImageSourceUrl, Size = ImageSize.Small, Height = 480, Width = 640 }]
        };

        await TestHelper.VerifySerializedObject(image, AlexaJson, "TemplateImage_WithSizeAndDimensions");
    }
}
