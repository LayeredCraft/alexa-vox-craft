using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.MediatR.Tests.Attributes;

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
    public async Task GetPersistentAsync_WithoutPersistenceAdapter_ThrowsInvalidOperationException(
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var exception = await Record.ExceptionAsync(() => manager.GetPersistentAsync(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
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
    public async Task GetPersistentAsync_CalledMultipleTimes_ReturnsSameBagInstance(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement>();
        persistenceAdapter.GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>())
            .Returns(adapterData);

        var first = await manager.GetPersistentAsync(TestContext.Current.CancellationToken);
        var second = await manager.GetPersistentAsync(TestContext.Current.CancellationToken);

        first.Should().BeSameAs(second);
    }

    [Theory]
    [MediatRAutoData]
    public async Task SavePersistentAttributes_WithoutPersistenceAdapter_ThrowsInvalidOperationException(
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var exception = await Record.ExceptionAsync(() => manager.SavePersistentAttributes(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
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
    public async Task GetSession_WhenSessionIsNull_ReturnsNull(
        [Frozen] SkillRequest skillRequest, SkillRequestFactory factory)
    {
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        var result = await manager.GetSession(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

}