using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Directive.Templates;
using AlexaVoxCraft.Model.Tests.Infrastructure;

namespace AlexaVoxCraft.Model.Tests.Directives;

public sealed class RenderTemplateTests
{
    private const string ExamplesPath = "Examples";
    private const string ImageSource = "https://example.com/resources/card-images/mount-saint-helen-small.png";
    private const string ImageDescription = "Mount St. Helens landscape";
    //Examples found at: https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/docs/display-interface-reference#configure-your-skill-for-the-display-directive

    [Fact]
    public void Create_Basic_Image()
    {
        var actual = new TemplateImage
        {
            ContentDescription = ImageDescription,
            Sources = new List<ImageSource> { new ImageSource { Url = ImageSource } }
        };
        Assert.True(Utility.CompareJson(actual, "TemplateImageBasic.json"));
    }

    [Fact]
    public void Create_Image()
    {
        var actual = new TemplateImage
        {
            ContentDescription = ImageDescription,
            Sources = new List<ImageSource> { new ImageSource {
                    Url = ImageSource,
                    Size = ImageSize.Small,
                    Height=480,
                    Width=640
                }
            }
        };
        Assert.True(Utility.CompareJson(actual, "TemplateImage.json"));
    }

    [Fact]
    public void HintCreatesCorrectJson()
    {
        var hint = new HintDirective("test hint");
        Assert.True(Utility.CompareJson(hint, "HintDirective.json"));
    }

}