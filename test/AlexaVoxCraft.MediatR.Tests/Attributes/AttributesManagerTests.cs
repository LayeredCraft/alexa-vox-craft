using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.MediatR.Tests.Attributes;

file sealed class TestSkillState
{
    public string CurrentIntent { get; set; } = string.Empty;
    public int TurnCount { get; set; }
}

public class AttributesManagerTests : TestBase
{
    [Theory]
    [MediatRAutoData]
    public void Constructor_WithValidFactory_CreatesInstance(SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        manager.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        var exception = Record.Exception(() => new AttributesManager(null!));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithFactoryReturningNull_ThrowsArgumentNullException(IPersistenceAdapter persistenceAdapter)
    {
        SkillRequestFactory factory = () => null!;

        var exception = Record.Exception(() => new AttributesManager(factory, persistenceAdapter));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public void Session_WhenSkillRequestHasSessionAttributes_ContainsThoseAttributes(
        [Frozen] SkillRequest skillRequest, SkillRequestFactory factory)
    {
        var key = "greeting";
        var element = JsonSerializer.SerializeToElement("hello", AlexaJsonOptions.DefaultOptions);
        skillRequest.Session = new Session { Attributes = new Dictionary<string, JsonElement> { [key] = element } };
        var manager = new AttributesManager(factory);

        manager.Session.Values.Should().ContainKey(key);
    }

    [Theory]
    [MediatRAutoData]
    public void Session_WhenSkillRequestHasNoSession_IsEmpty(
        [Frozen] SkillRequest skillRequest, SkillRequestFactory factory)
    {
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        manager.Session.Values.Should().BeEmpty();
    }

    [Theory]
    [MediatRAutoData]
    public void Request_IsInitiallyEmpty(AttributesManager manager)
    {
        manager.Request.Values.Should().BeEmpty();
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetPersistentAsync_WithoutPersistenceAdapter_ThrowsMissingMemberException(
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var exception = await Record.ExceptionAsync(() => manager.GetPersistentAsync(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetPersistentAsync_WithPersistenceAdapter_ReturnsAttributesFromAdapter(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        var key = "savedKey";
        var element = JsonSerializer.SerializeToElement(42, AlexaJsonOptions.DefaultOptions);
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement> { [key] = element };
        persistenceAdapter.GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>())
            .Returns(adapterData);

        var result = await manager.GetPersistentAsync(TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Values.Should().ContainKey(key);
        await persistenceAdapter.Received(1).GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetPersistentAsync_CalledMultipleTimes_CallsAdapterOnlyOnce(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement>();
        persistenceAdapter.GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>())
            .Returns(adapterData);

        await manager.GetPersistentAsync(TestContext.Current.CancellationToken);
        await manager.GetPersistentAsync(TestContext.Current.CancellationToken);

        await persistenceAdapter.Received(1).GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task SavePersistentAttributes_WithoutPersistenceAdapter_ThrowsMissingMemberException(
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var exception = await Record.ExceptionAsync(() => manager.SavePersistentAttributes(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task SavePersistentAttributes_WithoutLoadingFirst_DoesNotCallAdapter(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        await persistenceAdapter.DidNotReceive().SaveAttribute(Arg.Any<SkillRequest>(),
            Arg.Any<IDictionary<string, JsonElement>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task SavePersistentAttributes_AfterLoadingAttributes_CallsAdapter(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement>();
        persistenceAdapter.GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>())
            .Returns(adapterData);

        await manager.GetPersistentAsync(TestContext.Current.CancellationToken);
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        await persistenceAdapter.Received(1).SaveAttribute(Arg.Any<SkillRequest>(),
            Arg.Any<IDictionary<string, JsonElement>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetSession_ReturnsSkillRequestSession([Frozen] SkillRequest skillRequest,
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var result = await manager.GetSession(TestContext.Current.CancellationToken);

        result.Should().BeSameAs(skillRequest.Session);
    }

    [Theory]
    [MediatRAutoData]
    public void SetSessionState_ThenGetSessionState_RoundTripsValue(AttributesManager manager)
    {
        var state = new TestSkillState { CurrentIntent = "PlayMusic", TurnCount = 3 };

        manager.SetSessionState("myState", state);
        var result = manager.GetSessionState<TestSkillState>("myState");

        result.Should().NotBeNull();
        result!.CurrentIntent.Should().Be("PlayMusic");
        result.TurnCount.Should().Be(3);
    }

    [Theory]
    [MediatRAutoData]
    public void GetSessionState_WhenNotSet_ReturnsNull(AttributesManager manager)
    {
        var result = manager.GetSessionState<TestSkillState>("missing");

        result.Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public void TryGetSessionState_WhenNotSet_ReturnsFalse(AttributesManager manager)
    {
        var found = manager.TryGetSessionState<TestSkillState>("missing", out var result);

        found.Should().BeFalse();
        result.Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public void TryGetSessionState_WhenSet_ReturnsTrueAndValue(AttributesManager manager)
    {
        var state = new TestSkillState { CurrentIntent = "HelpIntent", TurnCount = 1 };
        manager.SetSessionState("myState", state);

        var found = manager.TryGetSessionState<TestSkillState>("myState", out var result);

        found.Should().BeTrue();
        result.Should().NotBeNull();
        result!.CurrentIntent.Should().Be("HelpIntent");
    }

    [Theory]
    [MediatRAutoData]
    public void ClearSessionState_RemovesKey(AttributesManager manager)
    {
        manager.SetSessionState("myState", new TestSkillState { TurnCount = 5 });

        manager.ClearSessionState("myState");

        manager.GetSessionState<TestSkillState>("myState").Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public void SetSessionState_StoresValueInSessionBag(AttributesManager manager)
    {
        var state = new TestSkillState { TurnCount = 7 };

        manager.SetSessionState("myState", state);

        manager.Session.Values.Should().ContainKey("myState");
    }
}