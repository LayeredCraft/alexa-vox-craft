using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

public class ContainerTests : TestBase
{
    [Fact]
    public async Task Container_WithCollectionExpression_Items_Serializes()
    {
        var container = new Container
        {
            Items = [
                new Text { Content = "First" },
                new Text { Content = "Second" },
                new Text { Content = "Third" }
            ]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "CollectionExpression");
    }

    [Fact]
    public async Task Container_WithSingleItem_SerializesAsObject()
    {
        var container = new Container
        {
            Items = [new Text { Content = "Single" }]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "SingleItem");
    }

    [Fact]
    public async Task Container_WithMultipleItems_SerializesAsArray()
    {
        var container = new Container
        {
            Items = [
                new Text { Content = "Item 1" },
                new Text { Content = "Item 2" }
            ]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "MultipleItems");
    }

    [Fact]
    public async Task Container_WithFirstItem_Serializes()
    {
        var container = new Container
        {
            FirstItem = [new Spacer { Height = "8dp" }]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "FirstItem");
    }

    [Fact]
    public async Task Container_WithLastItem_Serializes()
    {
        var container = new Container
        {
            LastItem = [new Spacer { Height = "8dp" }]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "LastItem");
    }

    [Fact]
    public async Task Container_WithFirstItemAndLastItem_Serializes()
    {
        var container = new Container
        {
            FirstItem = [new Spacer { Height = "8dp" }],
            Items = [
                new Text { Content = "Content 1" },
                new Text { Content = "Content 2" }
            ],
            LastItem = [new Spacer { Height = "16dp" }]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "FirstAndLastItem");
    }

    [Fact]
    public async Task Container_WithExpressionString_Items_Serializes()
    {
        var container = new Container
        {
            Items = "${payload.items}"
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "ExpressionItems");
    }

    [Fact]
    public async Task Container_WithExpressionString_FirstItem_Serializes()
    {
        var container = new Container
        {
            FirstItem = "${payload.firstItem}"
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "ExpressionFirstItem");
    }

    [Fact]
    public async Task Container_WithConstructor_Serializes()
    {
        var container = new Container(
            new Text { Content = "Constructed 1" },
            new Text { Content = "Constructed 2" }
        );

        await TestHelper.VerifySerializedObject(container, AlexaJson, "Constructor");
    }

    [Fact]
    public async Task Container_WithDirection_Serializes()
    {
        var container = new Container
        {
            Direction = "row",
            Items = [
                new Text { Content = "Left" },
                new Text { Content = "Right" }
            ]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "DirectionRow");
    }

    [Fact]
    public void Container_Deserializes_FromArrayItems()
    {
        const string json = """{"type":"Container","items":[{"type":"Text","text":"Item 1"},{"type":"Text","text":"Item 2"}]}""";

        var container = JsonSerializer.Deserialize<Container>(json, AlexaJson);

        container.Should().NotBeNull();
        container.Items.Should().NotBeNull();
        container.Items.Items.Should().HaveCount(2);
        container.Items.Items[0].Should().BeOfType<Text>();
        container.Items.Items[1].Should().BeOfType<Text>();
    }

    [Fact]
    public void Container_Deserializes_FromSingleObjectItem()
    {
        const string json = """{"type":"Container","items":{"type":"Text","text":"Single"}}""";

        var container = JsonSerializer.Deserialize<Container>(json, AlexaJson);

        container.Should().NotBeNull();
        container.Items.Should().NotBeNull();
        container.Items.Items.Should().HaveCount(1);
        container.Items.Items[0].Should().BeOfType<Text>();
    }

    [Fact]
    public void Container_Deserializes_FromExpressionItems()
    {
        const string json = """{"type":"Container","items":"${data.items}"}""";

        var container = JsonSerializer.Deserialize<Container>(json, AlexaJson);

        container.Should().NotBeNull();
        container.Items.Should().NotBeNull();
        container.Items.Expression.Should().Be("${data.items}");
        container.Items.Items.Should().BeEmpty();
    }

    [Fact]
    public void Container_Deserializes_WithFirstItemAndLastItem()
    {
        const string json = """{"type":"Container","firstItem":{"type":"Spacer","height":"8dp"},"lastItem":[{"type":"Spacer","height":"16dp"}]}""";

        var container = JsonSerializer.Deserialize<Container>(json, AlexaJson);

        container.Should().NotBeNull();
        container.FirstItem.Should().NotBeNull();
        container.FirstItem!.Items.Should().HaveCount(1);
        container.FirstItem.Items![0].Should().BeOfType<Spacer>();

        container.LastItem.Should().NotBeNull();
        container.LastItem.Items.Should().HaveCount(1);
        container.LastItem.Items![0].Should().BeOfType<Spacer>();
    }

    [Fact]
    public async Task Container_WithAllProperties_Serializes()
    {
        var container = new Container
        {
            Direction = "column",
            AlignItems = "center",
            JustifyContent = "spaceAround",
            Wrap = ContainerWrap.Wrap!,
            Numbered = true,
            FirstItem = [new Spacer { Height = "8dp" }],
            Items = [
                new Text { Content = "Content 1" },
                new Text { Content = "Content 2" },
                new Text { Content = "Content 3" }
            ],
            LastItem = [new Spacer { Height = "16dp" }]
        };

        await TestHelper.VerifySerializedObject(container, AlexaJson, "AllProperties");
    }
}