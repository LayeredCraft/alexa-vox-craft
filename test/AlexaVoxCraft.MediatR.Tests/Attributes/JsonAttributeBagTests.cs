using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.MediatR.Tests.Attributes;

file sealed class BagState
{
    public string CurrentIntent { get; set; } = string.Empty;
    public int TurnCount { get; set; }
}

public class JsonAttributeBagTests : TestBase
{
    [Fact]
    public void Constructor_WithNullDictionary_ThrowsArgumentNullException()
    {
        var exception = Record.Exception(() => new JsonAttributeBag(null!));

        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void Values_ReturnsUnderlyingDictionary()
    {
        var dict = new Dictionary<string, JsonElement>();
        var bag = new JsonAttributeBag(dict);

        bag.Values.Should().BeSameAs(dict);
    }

    [Fact]
    public void Set_ThenGet_RoundTripsTypedValue()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
        var state = new BagState { CurrentIntent = "PlayMusic", TurnCount = 3 };

        bag.Set("state", state);
        var result = bag.Get<BagState>("state");

        result.Should().NotBeNull();
        result!.CurrentIntent.Should().Be("PlayMusic");
        result.TurnCount.Should().Be(3);
    }

    [Fact]
    public void Set_StoresRawElementInValues()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());

        bag.Set("key", "hello");

        bag.Values.Should().ContainKey("key");
    }

    [Fact]
    public void Get_WhenKeyNotFound_ReturnsDefault()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());

        var result = bag.Get<BagState>("missing");

        result.Should().BeNull();
    }

    [Fact]
    public void Get_WhenValueIsValueType_ReturnsDefaultForMissingKey()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());

        var result = bag.Get<int>("missing");

        result.Should().Be(0);
    }

    [Fact]
    public void TryGet_WhenKeyNotFound_ReturnsFalseAndDefault()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());

        var found = bag.TryGet<BagState>("missing", out var result);

        found.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGet_WhenKeyFound_ReturnsTrueAndDeserializedValue()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
        var state = new BagState { CurrentIntent = "HelpIntent", TurnCount = 1 };
        bag.Set("state", state);

        var found = bag.TryGet<BagState>("state", out var result);

        found.Should().BeTrue();
        result.Should().NotBeNull();
        result!.CurrentIntent.Should().Be("HelpIntent");
        result.TurnCount.Should().Be(1);
    }

    [Fact]
    public void GetRequired_WhenKeyFound_ReturnsValue()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
        bag.Set("count", 42);

        var result = bag.GetRequired<int>("count");

        result.Should().Be(42);
    }

    [Fact]
    public void GetRequired_WhenKeyNotFound_ThrowsKeyNotFoundException()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());

        var exception = Record.Exception(() => bag.GetRequired<BagState>("missing"));

        exception.Should().BeOfType<KeyNotFoundException>();
    }

    [Fact]
    public void Indexer_Get_ReturnsRawElement()
    {
        var element = JsonSerializer.SerializeToElement("hello", AlexaJsonOptions.DefaultOptions);
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement> { ["key"] = element });

        bag["key"].GetString().Should().Be("hello");
    }

    [Fact]
    public void Indexer_Set_StoresElement()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
        var element = JsonSerializer.SerializeToElement(99, AlexaJsonOptions.DefaultOptions);

        bag["key"] = element;

        bag.Values.Should().ContainKey("key");
    }

    [Fact]
    public void Remove_WhenKeyExists_ReturnsTrueAndRemovesKey()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
        bag.Set("key", "value");

        var removed = bag.Remove("key");

        removed.Should().BeTrue();
        bag.Get<string>("key").Should().BeNull();
    }

    [Fact]
    public void Remove_WhenKeyDoesNotExist_ReturnsFalse()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());

        var removed = bag.Remove("missing");

        removed.Should().BeFalse();
    }

    [Fact]
    public void Clear_RemovesAllKeys()
    {
        var bag = new JsonAttributeBag(new Dictionary<string, JsonElement>());
        bag.Set("a", 1);
        bag.Set("b", 2);

        bag.Clear();

        bag.Values.Should().BeEmpty();
    }
}