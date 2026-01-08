using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class APLCollectionTests : TestBase
{
    private static JsonSerializerOptions CreateOptions(bool alwaysOutputArray)
    {
        var options = new JsonSerializerOptions(AlexaJson);
        options.Converters.Add(new APLCollectionConverter<APLComponent>(alwaysOutputArray));
        return options;
    }

    [Fact]
    public async Task APLCollection_WithCollectionExpression_Serializes()
    {
        // Collection expression syntax: [item1, item2]
        APLCollection<APLComponent> collection = [
            new Text { Content = "Hello" },
            new Spacer { Height = "16dp" },
            new Text { Content = "World" }
        ];

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "CollectionExpression");
    }

    [Fact]
    public async Task APLCollection_WithExpressionString_Serializes()
    {
        // Expression string assignment: "${data.items}"
        APLCollection<APLComponent> collection = "${data.items}";

        collection.Expression.Should().Be("${data.items}");

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "Expression");
    }

    [Fact]
    public async Task APLCollection_ImplicitFromList_Serializes()
    {
        // Implicit conversion from List<T>
        var list = new List<APLComponent>
        {
            new Text { Content = "Item 1" },
            new Text { Content = "Item 2" }
        };

        APLCollection<APLComponent> collection = list;

        collection.Items.Should().HaveCount(2);

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "ImplicitFromList");
    }

    [Fact]
    public async Task APLCollection_ImplicitFromArray_Serializes()
    {
        // Implicit conversion from T[]
        APLComponent[] array = [new Text { Content = "Array Item" }];
        APLCollection<APLComponent> collection = array;

        collection.Items.Should().HaveCount(1);

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "ImplicitFromArray");
    }

    [Fact]
    public async Task APLCollection_AlwaysOutputArrayFalse_SingleItem_SerializesAsObject()
    {
        // AlwaysOutputArray=false: single-item as object
        APLCollection<APLComponent> collection = [new Text { Content = "Single" }];

        await TestHelper.VerifySerializedObject(collection, CreateOptions(false), "SingleItemAsObject");
    }

    [Fact]
    public async Task APLCollection_AlwaysOutputArrayTrue_SingleItem_SerializesAsArray()
    {
        // AlwaysOutputArray=true: single-item as array
        APLCollection<APLComponent> collection = [new Text { Content = "Single" }];

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "SingleItemAsArray");
    }

    [Fact]
    public void APLCollection_SupportsLinq()
    {
        // LINQ operations via IEnumerable<T>
        APLCollection<APLComponent> collection = [
            new Text { Content = "Text 1" },
            new Spacer { Height = "16dp" },
            new Text { Content = "Text 2" }
        ];

        var textComponents = collection.OfType<Text>().ToList();
        var spacerComponents = collection.OfType<Spacer>().ToList();

        textComponents.Should().HaveCount(2);
        spacerComponents.Should().HaveCount(1);
    }

    [Fact]
    public void APLCollection_Deserializes_FromArray()
    {
        var json = """[{"type":"Text","text":"Item 1"},{"type":"Text","text":"Item 2"}]""";

        var collection = JsonSerializer.Deserialize<APLCollection<APLComponent>>(json, CreateOptions(true));

        collection.Should().NotBeNull();
        collection!.Items.Should().HaveCount(2);
        collection.Items![0].Should().BeOfType<Text>();
        collection.Items[1].Should().BeOfType<Text>();
        collection.Expression.Should().BeNullOrEmpty();
    }

    [Fact]
    public void APLCollection_Deserializes_FromSingleObject()
    {
        var json = """{"type":"Text","text":"Single Item"}""";

        var collection = JsonSerializer.Deserialize<APLCollection<APLComponent>>(json, CreateOptions(false));

        collection.Should().NotBeNull();
        collection!.Items.Should().HaveCount(1);
        collection.Items![0].Should().BeOfType<Text>();
        collection.Expression.Should().BeNullOrEmpty();
    }

    [Fact]
    public void APLCollection_Deserializes_FromExpression()
    {
        var json = "\"${data.items}\"";

        var collection = JsonSerializer.Deserialize<APLCollection<APLComponent>>(json, CreateOptions(true));

        collection.Should().NotBeNull();
        collection!.Expression.Should().Be("${data.items}");
        collection.Items.Should().BeEmpty();
    }

    [Fact]
    public void APLCollection_NullHandling()
    {
        List<APLComponent>? nullList = null;
        string? nullExpression = null;

        // Implicit conversions with null
        APLCollection<APLComponent>? fromNullList = nullList;
        APLCollection<APLComponent>? fromNullExpression = nullExpression;

        fromNullList.Should().BeNull();
        fromNullExpression.Should().BeNull();
    }
}