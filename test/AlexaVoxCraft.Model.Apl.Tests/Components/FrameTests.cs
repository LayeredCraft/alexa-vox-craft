using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class FrameTests : TestBase
{
    [Fact]
    public async Task Frame_WithCollectionExpression_SingleItem_Serializes()
    {
        var frame = new Frame
        {
            Item = [new Text { Content = "Framed Text" }]
        };

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "CollectionExpressionSingle");
    }

    [Fact]
    public async Task Frame_WithCollectionExpression_MultipleItems_Serializes()
    {
        var frame = new Frame
        {
            Item = [
                new Text { Content = "First" },
                new Text { Content = "Second" }
            ]
        };

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "CollectionExpressionMultiple");
    }

    [Fact]
    public async Task Frame_WithSingleConstructor_Serializes()
    {
        var frame = new Frame(new Text { Content = "Constructed" });

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "SingleConstructor");
    }

    [Fact]
    public async Task Frame_WithParamsConstructor_Serializes()
    {
        var frame = new Frame(
            new Text { Content = "Item 1" },
            new Text { Content = "Item 2" }
        );

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "ParamsConstructor");
    }

    [Fact]
    public async Task Frame_WithExpressionString_Item_Serializes()
    {
        var frame = new Frame
        {
            Item = "${payload.item}"
        };

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "ExpressionItem");
    }

    [Fact]
    public async Task Frame_WithBackgroundColor_Serializes()
    {
        var frame = new Frame
        {
            BackgroundColor = "#FF0000",
            Item = [new Text { Content = "Red Frame" }]
        };

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "BackgroundColor");
    }

    [Fact]
    public async Task Frame_WithBorderRadius_Serializes()
    {
        var frame = new Frame
        {
            BorderRadius = "8dp",
            BorderWidth = 2,
            BorderColor = "#000000",
            Item = [new Text { Content = "Bordered" }]
        };

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "BorderRadius");
    }

    [Fact]
    public async Task Frame_WithIndividualCornerRadius_Serializes()
    {
        var frame = new Frame
        {
            BorderTopLeftRadius = "4dp",
            BorderTopRightRadius = "8dp",
            BorderBottomLeftRadius = "12dp",
            BorderBottomRightRadius = "16dp",
            Item = [new Text { Content = "Custom Corners" }]
        };

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "IndividualCorners");
    }

    [Fact]
    public void Frame_Deserializes_FromSingleObjectItem()
    {
        const string json = """{"type":"Frame","item":{"type":"Text","text":"Single"}}""";

        var frame = JsonSerializer.Deserialize<Frame>(json, AlexaJson);

        frame.Should().NotBeNull();
        frame.Item.Should().NotBeNull();
        frame.Item!.Items.Should().HaveCount(1);
        frame.Item.Items![0].Should().BeOfType<Text>();
    }

    [Fact]
    public void Frame_Deserializes_FromArrayItem()
    {
        const string json = """{"type":"Frame","item":[{"type":"Text","text":"Item 1"},{"type":"Text","text":"Item 2"}]}""";

        var frame = JsonSerializer.Deserialize<Frame>(json, AlexaJson);

        frame.Should().NotBeNull();
        frame.Item.Should().NotBeNull();
        frame.Item!.Items.Should().HaveCount(2);
        frame.Item.Items![0].Should().BeOfType<Text>();
        frame.Item.Items[1].Should().BeOfType<Text>();
    }

    [Fact]
    public void Frame_Deserializes_FromExpressionItem()
    {
        const string json = """{"type":"Frame","item":"${data.item}"}""";

        var frame = JsonSerializer.Deserialize<Frame>(json, AlexaJson);

        frame.Should().NotBeNull();
        frame.Item.Should().NotBeNull();
        frame.Item!.Expression.Should().Be("${data.item}");
        frame.Item.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Frame_WithAllBorderProperties_Serializes()
    {
        var frame = new Frame
        {
            BackgroundColor = "#FFFFFF",
            BorderWidth = 3,
            BorderColor = "#0000FF",
            BorderRadius = "10dp",
            BorderStrokeWidth = "2dp",
            Item = [new Text { Content = "Complete Border" }]
        };

        await TestHelper.VerifySerializedObject(frame, AlexaJson, "AllBorderProperties");
    }

    [Fact]
    public async Task Frame_NestedInContainer_Serializes()
    {
        var container = new Container
        {
            Items = [
                new Frame
                {
                    BackgroundColor = "#FF0000",
                    BorderRadius = "8dp",
                    Item = [new Text { Content = "Red Frame" }]
                },
                new Frame
                {
                    BackgroundColor = "#00FF00",
                    BorderRadius = "8dp",
                    Item = [new Text { Content = "Green Frame" }]
                }
            ]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "NestedInContainer");
    }

    [Fact]
    public void Frame_Deserializes_WithBorderProperties()
    {
        const string json = """{"type":"Frame","backgroundColor":"#FFFFFF","borderColor":"#000000","borderWidth":2,"borderRadius":"8dp","item":{"type":"Text","text":"Bordered"}}""";

        var frame = JsonSerializer.Deserialize<Frame>(json, AlexaJson);

        frame.Should().NotBeNull();
        frame.BackgroundColor.Should().NotBeNull();
        frame.BackgroundColor!.Value.Should().Be("#FFFFFF");
        frame.BorderColor.Should().NotBeNull();
        frame.BorderColor!.Value.Should().Be("#000000");
        frame.BorderWidth.Should().NotBeNull();
        frame.BorderWidth!.Value.Should().Be(2);
        frame.BorderRadius.Should().NotBeNull();
        frame.Item.Should().NotBeNull();
        frame.Item!.Items.Should().HaveCount(1);
    }
}