using System.Linq;
using AlexaVoxCraft.Model.Request;
using Xunit;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class LaunchRequestTests
{
    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void AbleToConfirmInterfaceName()
    {
        var request = Utility.ExampleFileContent<SkillRequest>("LaunchRequest.json");
        Assert.True(request.Context.System.Device.SupportedInterfaces.ContainsKey(APLInterface.InterfaceName));

        var result = request.Context.System.Device.SupportedInterfaces[APLInterface.InterfaceName];
        Assert.NotNull(result);
    }

    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void SupportsHelperMethodConfirms()
    {
        var request = Utility.ExampleFileContent<SkillRequest>("LaunchRequest.json");
        APLInterface.Supported(request);
    }

    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void APLInterfaceDetailsReturnsVersion()
    {
        var request = Utility.ExampleFileContent<SkillRequest>("LaunchRequest.json");
        Assert.Equal(APLDocumentVersion.Unknown,request.APLInterfaceDetails().Runtime.MaxVersion);
        Assert.Equal("1.21", request.APLInterfaceDetails().Runtime.MaxVersionString);
    }

    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void MainViewportDeserializesCorrectly()
    {
        var request = Utility.ExampleFileContent<APLSkillRequest>("LaunchRequest.json");
        Assert.Equal(ViewportShape.Rectangle, request.Context.Viewport.Shape);
        Assert.Equal(160, request.Context.Viewport.DPI);
    }

    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void ViewportArrayTypeDeserializesCorrectly()
    {
        var request = Utility.ExampleFileContent<APLSkillRequest>("LaunchRequest.json");
        Assert.Equal(2,request.Context.Viewports.Length);
        Assert.Single(request.Context.Viewports.OfType<APLViewport>());
        Assert.Single(request.Context.Viewports.OfType<APLTViewport>());
    }

    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void APLViewportPropertiesSetCorrectly()
    {
        var request = Utility.ExampleFileContent<APLSkillRequest>("LaunchRequest.json");
        var viewport = request.Context.Viewports.OfType<APLViewport>().First();
        Assert.Equal(ViewportShape.Rectangle,viewport.Shape);
        Assert.Equal(160, viewport.DPI);
        Assert.Equal("STANDARD", viewport.PresentationType);
        Assert.False(viewport.CanRotate);

        Assert.NotNull(viewport.Configuration.Current);
        var config = viewport.Configuration.Current;
        Assert.NotNull(config.Video);
        Assert.Equal(2,config.Video.Codecs.Length);

        Assert.Equal("DISCRETE",config.Size.Type);
        Assert.Equal(1024, config.Size.PixelWidth);
        Assert.Equal(600, config.Size.PixelHeight);
    }

    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void APLTViewportPropertiesSetCorrectly()
    {
        var request = Utility.ExampleFileContent<APLSkillRequest>("LaunchRequest.json");
        var viewport = request.Context.Viewports.OfType<APLTViewport>().First();
            
        var profile = Assert.Single(viewport.SupportedProfiles);
        Assert.Equal(APLTProfile.FourCharacterClock,profile);
        Assert.Equal(4,viewport.LineLength);
        Assert.Equal(1,viewport.LineCount);
        Assert.Equal(APLTFormat.SevenSegment,viewport.Format);
        var segment = Assert.Single(viewport.InterSegments);
        Assert.Equal(2,segment.X);
        Assert.Equal(0,segment.Y);
        Assert.Equal(3,segment.Characters.Length);
    }

    [Fact(Skip = "Temporarily skipping due to CI issues")]
    public void APLContextInformation()
    {
        var request = Utility.ExampleFileContent<APLSkillRequest>("LaunchRequest.json");
        var info = request.Context.AplVisualContext;
        Assert.Equal("widget://amzn1.ask.skill.1/MyWidgetSandbox/f1cf2427-1-1-1-1", info.PresentationUri);
        Assert.Equal("helloworldWithButtonToken", info.Token);
        Assert.Equal("APL_WEB_RENDERER_GANDALF", info.Version);
        var component = Assert.Single(info.ComponentsVisibleOnScreen);

        Assert.Equal(":1000", component.Uid);
        Assert.Equal("text", component.Type);
        Assert.Equal("1024x600+0+0:0", component.Position);
        Assert.Null(component.Tags.Viewport);
        var child = Assert.Single(component.Children);

        Assert.Equal("fadeHelloTextButton",child.Id);
        Assert.Equal(":1002",child.Uid);
        Assert.True(child.Tags.Clickable);
        Assert.False(child.Tags.Focused);
    }
}