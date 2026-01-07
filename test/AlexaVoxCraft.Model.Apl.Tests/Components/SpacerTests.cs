using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class SpacerTests : TestBase
{
    [Fact]
    public async Task Spacer_WithHeight_Serializes()
    {
        var spacer = new Spacer
        {
            Height = "16dp"
        };

        var json = JsonSerializer.Serialize(spacer, AlexaJson);
        await Verify(json);
    }

    [Fact]
    public async Task Spacer_WithHeightAndWidth_Serializes()
    {
        var spacer = new Spacer
        {
            Height = "16dp",
            Width = "100%"
        };

        var json = JsonSerializer.Serialize(spacer, AlexaJson);
        await Verify(json);
    }

    [Fact]
    public async Task Spacer_InContainer_Serializes()
    {
        var container = new Container
        {
            Items = new List<APLComponent>
            {
                new Text { Content = "Hello" },
                new Spacer { Height = "16dp" },
                new Text { Content = "World" }
            }
        };

        var json = JsonSerializer.Serialize(container, AlexaJson);
        await Verify(json);
    }
}