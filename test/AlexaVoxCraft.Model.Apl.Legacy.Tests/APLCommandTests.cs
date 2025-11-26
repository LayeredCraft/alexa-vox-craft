using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Legacy.Tests;

public class APLCommandTests
{
    private readonly ITestOutputHelper _output;

    public APLCommandTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CommandDefinitionWorksProperly()
    {
        var commandDefinition = Utility.ExampleFileContent<CommandDefinition>("jackinthebox.json");
        Assert.Equal(2,commandDefinition.Parameters.Value.Count);
        var item = Assert.IsType<AnimateItem>(commandDefinition.Commands.Value.First());
        var transform = Assert.IsType<AnimatedTransform>(item.Value.Value.Skip(1).First());
        Assert.Equal(2,transform.From.Value.Count);
        Assert.Equal(0.1,transform.From.Value.First().ScaleX.Value);
    }

    [Fact]
    public void AnimatedItemWorksProperly()
    {
        var list = new List<AnimatedProperty>
        {
            new AnimatedOpacity
            {
                From = 0,
                To = 1
            }
        };

        var command = new AnimateItem
        {
            Duration = 1000,
            RepeatCount = 9,
            RepeatMode = RepeatMode.Reverse,
            Value = list
        };

        Assert.True(Utility.CompareJson(command,"AnimatedItem.json", _output));

        var deserial = Utility.ExampleFileContent<AnimateItem>("AnimatedItem.json");
        var property = Assert.Single(deserial.Value.Value);
        var opacity = Assert.IsType<AnimatedOpacity>(property);
        Assert.Equal(0,opacity.From.Value);
        Assert.Equal(1,opacity.To.Value);
    }

    [Fact]
    public void SpeakItemWorksProperly()
    {
        var command = new SpeakItem
        {
            HighlightMode = HighlightMode.Line,
            Align = ItemAlignment.Center,
            ComponentId = "myJokeSetup"
        };

        var parsed = Utility.ExampleFileContent<SpeakItem>("SpeakItem.json");
        Assert.NotNull(parsed);
        Assert.True(Utility.CompareJson(command, "SpeakItem.json", null));
    }

    [Fact]
    public void PlayMediaWorksCorrectly()
    {
        var command = new ControlMedia
        {
            Command = ControlMediaCommand.Seek!,
            ComponentId = "myAudioPlayer",
            Value = 5000
        };
        Assert.True(Utility.CompareJson(command,"ControlMediaCommand.json", null));
    }

    [Fact]
    public void SetValueWorksCorrectly()
    {
        var command = new SetValue
        {
            ComponentId = "jokePunchline",
            Property = "opacity",
            Value = "1"
        };
        Assert.True(Utility.CompareJson(command,"SetValueCommand.json", null));
    }

    [Fact]
    public void PlayMediaDeserializesCorrectly()
    {
        var command = Utility.ExampleFileContent<ControlMedia>("ControlMediaCommand.json");
        Assert.Equal(ControlMediaCommand.Seek,command.Command.Value);
        Assert.Equal("myAudioPlayer",command.ComponentId.Value);
        Assert.Equal(5000,command.Value.Value);
    }

    [Fact]
    public void Finish()
    {
        // Arrange: create instance of Finish command
        var command = new Finish();

        // Act: serialize to JsonNode
        var jsonNode = JsonSerializer.SerializeToNode(command, AlexaJsonOptions.DefaultOptions);

        // Assert: it only has one property
        Assert.NotNull(jsonNode);
        var obj = Assert.IsType<JsonObject>(jsonNode);
        Assert.Single(obj);

        // Assert: type is "Finish"
        Assert.Equal("Finish", obj["type"]?.GetValue<string>());

        // Act: deserialize back into base APLCommand
        var deserialized = JsonSerializer.Deserialize<APLCommand>(jsonNode.ToJsonString(), AlexaJsonOptions.DefaultOptions);

        // Assert: it's correctly deserialized into the Finish type
        Assert.IsType<Finish>(deserialized);
    }

    [Fact]
    public void Reinflate()
    {
        // Arrange
        var command = new Reinflate { PreservedSequencers = new[] { "test" } };

        // Act: Serialize to JsonNode
        var node = JsonSerializer.SerializeToNode(command, AlexaJsonOptions.DefaultOptions);

        // Assert: Top-level object and property count
        var obj = Assert.IsType<JsonObject>(node);
        Assert.Equal(2, obj.Count);

        // Assert: "type" is "Reinflate"
        Assert.Equal("Reinflate", obj["type"]?.GetValue<string>());

        // Assert: preservedSequencers is a JSON array
        var preserved = obj["preservedSequencers"];
        Assert.NotNull(preserved);
        var array = Assert.IsType<JsonArray>(preserved);
        var sequencer = Assert.Single(array);
        Assert.Equal("test", sequencer?.GetValue<string>());

        // Act: Deserialize back into base APLCommand
        var deserialized = JsonSerializer.Deserialize<APLCommand>(node.ToJsonString(), AlexaJsonOptions.DefaultOptions);

        // Assert: deserialized type is correct
        Assert.IsType<Reinflate>(deserialized);
    }

    [Fact]
    public void Select()
    {
        var select = Utility.ExampleFileContent<Select>("Select.json");
        Assert.Single(select.Commands.Value);
        Assert.Equal(5, select.Data.Value.Count);
    }

    [Fact]
    public void InsertItem()
    {
        Utility.AssertSerialization<APLCommand, InsertItem>("Command_InsertItem.json", _output);
    }

    [Fact]
    public void RemoveItem()
    {
        Utility.AssertSerialization<APLCommand, RemoveItem>("Command_RemoveItem.json", _output);
    }

    [Fact]
    public void ScrollToComponent()
    {
        Utility.AssertSerialization<APLCommand, ScrollToComponent>("Command_ScrollToComponent.json", _output);
    }

    [Fact]
    public void SetPage()
    {
        Utility.AssertSerialization<APLCommand, SetPage>("Command_SetPage.json");
    }
}