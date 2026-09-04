using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Component-level coverage for <see cref="Text"/>, <see cref="TimeText"/>, and general dimension/
/// binding value handling shared across all components. Components are response objects the skill
/// only ever builds and sends, so they are tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public class TextTests : TestBase<TextTests>
{
    [Fact]
    public async Task Text_WithConstructor_Serializes()
    {
        var text = new Text("APL in C#") { FontSize = "24dp", TextAlign = "Center" };

        await TestHelper.VerifySerializedObject(text, AlexaJson, "Constructor");
    }

    [Fact]
    public void Text_DimensionAndBindingValues_SerializeToExpectedStrings()
    {
        var text = new Text("Hello World")
        {
            Color = APLValue.To<string>("${color}"),
            Disabled = APLValue.To<bool?>("${disabled}"),
            FontSize = "24dp",
            Left = new AbsoluteDimension(24, "vw"),
            PaddingLeft = new RelativeDimension(5),
            Top = "${top}",
            Right = new APLDimensionValue(new AbsoluteDimension(345, "dp")),
            Bottom = new APLDimensionValue("test")
        };

        var json = JsonSerializer.Serialize(text, AlexaJsonOptions.DefaultOptions);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("fontSize").GetString().Should().Be("24dp");
        root.GetProperty("left").GetString().Should().Be("24vw");
        root.GetProperty("paddingLeft").GetString().Should().Be("5%");
        root.GetProperty("top").GetString().Should().Be("${top}");
        root.GetProperty("right").GetString().Should().Be("345dp");
        root.GetProperty("bottom").GetString().Should().Be("test");
    }

    [Fact]
    public async Task TimeText_Serializes()
    {
        var timeText = new TimeText
        {
            Direction = TimeTextDirection.Down,
            Format = "%M:%S",
            Start = 1552070232
        };

        await TestHelper.VerifySerializedObject(timeText, AlexaJson, "TimeText");
    }

    [Fact]
    public async Task Component_WithBindings_Serializes()
    {
        var text = new Text("Hello")
        {
            Bindings = [new Binding("foo", "27"), new Binding("bar", "${foo + 23}")]
        };

        await TestHelper.VerifySerializedObject(text, AlexaJson, "WithBindings");
    }
}
