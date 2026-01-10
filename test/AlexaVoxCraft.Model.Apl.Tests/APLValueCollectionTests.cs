using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl.Tests;

public class AplValueCollectionTests : TestBase
{
    private static JsonSerializerOptions CreateOptions(bool alwaysOutputArray)
    {
        var options = new JsonSerializerOptions(AlexaJson);
        options.Converters.Add(new APLValueCollectionConverter<APLComponent>(alwaysOutputArray));
        return options;
    }

    [Fact]
    public async Task APLValueCollection_WithCollectionExpression_Serializes()
    {
        // Collection expression syntax: [item1, item2]
        APLValueCollection<APLComponent> collection = [
            new Text { Content = "Hello"! },
            new Spacer { Height = "16dp" },
            new Text { Content = "World"! }
        ];

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "CollectionExpression");
    }

    [Fact]
    public async Task APLValueCollection_WithExpressionString_Serializes()
    {
        // Expression string assignment: "${data.items}"
        APLValueCollection<APLComponent> collection = "${data.items}"!;

        collection.Expression.Should().Be("${data.items}");

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "Expression");
    }

    [Fact]
    public async Task APLValueCollection_ImplicitFromList_Serializes()
    {
        // Implicit conversion from List<T>
        var list = new List<APLComponent>
        {
            new Text { Content = "Item 1"! },
            new Text { Content = "Item 2"! }
        };

        APLValueCollection<APLComponent> collection = list!;

        collection.Should().HaveCount(2);

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "ImplicitFromList");
    }

    [Fact]
    public async Task APLValueCollection_ImplicitFromArray_Serializes()
    {
        // Implicit conversion from T[]
        APLComponent[] array = [new Text { Content = "Array Item"! }];
        APLValueCollection<APLComponent> collection = array!;

        collection.Should().HaveCount(1);

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "ImplicitFromArray");
    }

    [Fact]
    public async Task APLValueCollection_AlwaysOutputArrayFalse_SingleItem_SerializesAsObject()
    {
        // AlwaysOutputArray=false: single-item as object
        APLValueCollection<APLComponent> collection = [new Text { Content = "Single"! }];

        await TestHelper.VerifySerializedObject(collection, CreateOptions(false), "SingleItemAsObject");
    }

    [Fact]
    public async Task APLValueCollection_AlwaysOutputArrayTrue_SingleItem_SerializesAsArray()
    {
        // AlwaysOutputArray=true: single-item as array
        APLValueCollection<APLComponent> collection = [new Text { Content = "Single"! }];

        await TestHelper.VerifySerializedObject(collection, CreateOptions(true), "SingleItemAsArray");
    }

    [Fact]
    public void APLValueCollection_SupportsLinq()
    {
        // LINQ operations via IEnumerable<T>
        APLValueCollection<APLComponent> collection = [
            new Text { Content = "Text 1"! },
            new Spacer { Height = "16dp" },
            new Text { Content = "Text 2"! }
        ];

        var textComponents = collection.OfType<Text>().ToList();
        var spacerComponents = collection.OfType<Spacer>().ToList();

        textComponents.Should().HaveCount(2);
        spacerComponents.Should().HaveCount(1);
    }

    [Fact]
    public void APLValueCollection_Deserializes_FromArray()
    {
        const string json = """[{"type":"Text","text":"Item 1"},{"type":"Text","text":"Item 2"}]""";

        var collection = JsonSerializer.Deserialize<APLValueCollection<APLComponent>>(json, CreateOptions(true));

        collection.Should().NotBeNull();
        collection.Should().HaveCount(2);
        collection![0].Should().BeOfType<Text>();
        collection[1].Should().BeOfType<Text>();
        collection.Expression.Should().BeNullOrEmpty();
    }

    [Fact]
    public void APLValueCollection_Deserializes_FromSingleObject()
    {
        const string json = """{"type":"Text","text":"Single Item"}""";

        var collection = JsonSerializer.Deserialize<APLValueCollection<APLComponent>>(json, CreateOptions(false));

        collection.Should().NotBeNull();
        collection.Should().HaveCount(1);
        collection![0].Should().BeOfType<Text>();
        collection.Expression.Should().BeNullOrEmpty();
    }

    [Fact]
    public void APLValueCollection_Deserializes_FromExpression()
    {
        const string json = "\"${data.items}\"";

        var collection = JsonSerializer.Deserialize<APLValueCollection<APLComponent>>(json, CreateOptions(true));

        collection.Should().NotBeNull();
        collection!.Expression.Should().Be("${data.items}");
        collection.Should().BeEmpty();
    }

    [Fact]
    public void APLValueCollection_NullHandling()
    {
        List<APLComponent>? nullList = null;
        string? nullExpression = null;

        // Implicit conversions with null
        APLValueCollection<APLComponent>? fromNullList = nullList;
        APLValueCollection<APLComponent>? fromNullExpression = nullExpression;

        fromNullList.Should().BeNull();
        fromNullExpression.Should().BeNull();
    }

    #region IList<T> Interface Tests

    [Fact]
    public void APLValueCollection_IList_Count()
    {
        APLValueCollection<APLComponent> collection = [new Text(), new Spacer()];
        collection.Count.Should().Be(2);
    }

    [Fact]
    public void APLValueCollection_IList_IndexerGet()
    {
        var text = new Text { Content = "Test"! };
        APLValueCollection<APLComponent> collection = [text];
        collection[0].Should().Be(text);
    }

    [Fact]
    public void APLValueCollection_IList_IndexerSet()
    {
        APLValueCollection<APLComponent> collection = [new Text()];
        var newText = new Text { Content = "Updated"! };
        collection[0] = newText;
        collection[0].Should().Be(newText);
    }

    [Fact]
    public void APLValueCollection_IList_Add()
    {
        APLValueCollection<APLComponent> collection = [];
        var text = new Text();
        collection.Add(text);
        collection.Should().HaveCount(1);
        collection[0].Should().Be(text);
    }

    [Fact]
    public void APLValueCollection_IList_Remove()
    {
        var text = new Text();
        APLValueCollection<APLComponent> collection = [text];
        var removed = collection.Remove(text);
        removed.Should().BeTrue();
        collection.Should().BeEmpty();
    }

    [Fact]
    public void APLValueCollection_IList_Clear()
    {
        APLValueCollection<APLComponent> collection = [new Text(), new Spacer()];
        collection.Clear();
        collection.Should().BeEmpty();
    }

    [Fact]
    public void APLValueCollection_IList_Contains()
    {
        var text = new Text();
        APLValueCollection<APLComponent> collection = [text];
        collection.Contains(text).Should().BeTrue();
        collection.Contains(new Spacer()).Should().BeFalse();
    }

    [Fact]
    public void APLValueCollection_IList_IndexOf()
    {
        var text = new Text();
        APLValueCollection<APLComponent> collection = [new Spacer(), text];
        collection.IndexOf(text).Should().Be(1);
        collection.IndexOf(new Text()).Should().Be(-1);
    }

    [Fact]
    public void APLValueCollection_IList_Insert()
    {
        APLValueCollection<APLComponent> collection = [new Text()];
        var spacer = new Spacer();
        collection.Insert(0, spacer);
        collection.Should().HaveCount(2);
        collection[0].Should().Be(spacer);
    }

    [Fact]
    public void APLValueCollection_IList_RemoveAt()
    {
        APLValueCollection<APLComponent> collection = [new Text(), new Spacer()];
        collection.RemoveAt(0);
        collection.Should().HaveCount(1);
        collection[0].Should().BeOfType<Spacer>();
    }

    [Fact]
    public void APLValueCollection_IList_CopyTo()
    {
        var text = new Text();
        var spacer = new Spacer();
        APLValueCollection<APLComponent> collection = [text, spacer];
        var array = new APLComponent[2];
        collection.CopyTo(array, 0);
        array[0].Should().Be(text);
        array[1].Should().Be(spacer);
    }

    [Fact]
    public void APLValueCollection_IList_IsReadOnly()
    {
        APLValueCollection<APLComponent> collection = [];
        collection.IsReadOnly.Should().BeFalse();
    }

    #endregion

    #region Materialize() Behavior Tests

    [Fact]
    public void APLValueCollection_Add_ClearsExpression()
    {
        APLValueCollection<APLComponent> collection = "${data.items}"!;
        collection.Expression.Should().Be("${data.items}");

        collection.Add(new Text());

        collection.Expression.Should().BeNullOrEmpty();
        collection.Should().HaveCount(1);
    }

    [Fact]
    public void APLValueCollection_Remove_ClearsExpression()
    {
        var text = new Text();
        APLValueCollection<APLComponent> collection = [text];
        collection.Expression = "${data.items}";

        collection.Remove(text);

        collection.Expression.Should().BeNullOrEmpty();
        collection.Should().BeEmpty();
    }

    [Fact]
    public void APLValueCollection_Clear_ClearsExpression()
    {
        APLValueCollection<APLComponent> collection = [new Text()];
        collection.Expression = "${data.items}";

        collection.Clear();

        collection.Expression.Should().BeNullOrEmpty();
        collection.Should().BeEmpty();
    }

    [Fact]
    public void APLValueCollection_IndexerSet_ClearsExpression()
    {
        APLValueCollection<APLComponent> collection = [new Text()];
        collection.Expression = "${data.items}";

        collection[0] = new Spacer();

        collection.Expression.Should().BeNullOrEmpty();
        collection[0].Should().BeOfType<Spacer>();
    }

    [Fact]
    public void APLValueCollection_Insert_ClearsExpression()
    {
        APLValueCollection<APLComponent> collection = [new Text()];
        collection.Expression = "${data.items}";

        collection.Insert(0, new Spacer());

        collection.Expression.Should().BeNullOrEmpty();
        collection.Should().HaveCount(2);
    }

    [Fact]
    public void APLValueCollection_RemoveAt_ClearsExpression()
    {
        APLValueCollection<APLComponent> collection = [new Text(), new Spacer()];
        collection.Expression = "${data.items}";

        collection.RemoveAt(0);

        collection.Expression.Should().BeNullOrEmpty();
        collection.Should().HaveCount(1);
    }

    [Fact]
    public void APLValueCollection_Count_DoesNotClearExpression()
    {
        APLValueCollection<APLComponent> collection = "${data.items}"!;
        collection.Expression.Should().Be("${data.items}");

        var count = collection.Count;

        collection.Expression.Should().Be("${data.items}");
        count.Should().Be(0);
    }

    [Fact]
    public void APLValueCollection_Contains_DoesNotClearExpression()
    {
        APLValueCollection<APLComponent> collection = "${data.items}"!;
        collection.Expression.Should().Be("${data.items}");

        var contains = collection.Contains(new Text());

        collection.Expression.Should().Be("${data.items}");
        contains.Should().BeFalse();
    }

    [Fact]
    public void APLValueCollection_IndexOf_DoesNotClearExpression()
    {
        APLValueCollection<APLComponent> collection = "${data.items}"!;
        collection.Expression.Should().Be("${data.items}");

        var indexOf = collection.IndexOf(new Text());

        collection.Expression.Should().Be("${data.items}");
        indexOf.Should().Be(-1);
    }

    [Fact]
    public void APLValueCollection_ItemsPropertyAccess_DoesNotClearExpression()
    {
        APLValueCollection<APLComponent> collection = "${data.items}"!;
        collection.Expression.Should().Be("${data.items}");

        var items = collection.Items;

        collection.Expression.Should().Be("${data.items}");
        items.Should().BeEmpty();
    }

    #endregion
}