using Compono;
using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.MediatR.Tests.Attributes;

public class AttributesManagerTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
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
    [Compose<MediatRTestProfile>]
    public void Constructor_WithFactoryReturningNull_ThrowsArgumentNullException(IPersistenceAdapter persistenceAdapter)
    {
        SkillRequestFactory factory = () => null!;

        var exception = Record.Exception(() => new AttributesManager(factory, persistenceAdapter));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Session_WhenSkillRequestHasSessionAttributes_ContainsThoseAttributes(
        [Shared] SkillRequest skillRequest, SkillRequestFactory factory)
    {
        var key = "greeting";
        var element = JsonSerializer.SerializeToElement("hello", AlexaJsonOptions.DefaultOptions);
        skillRequest.Session = new Session { Attributes = new Dictionary<string, JsonElement> { [key] = element } };
        var manager = new AttributesManager(factory);

        manager.Session.Values.Should().ContainKey(key);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Session_WhenSkillRequestHasNoSession_IsEmpty(
        [Shared] SkillRequest skillRequest, SkillRequestFactory factory)
    {
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        manager.Session.Values.Should().BeEmpty();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Request_IsInitiallyEmpty(AttributesManager manager)
    {
        manager.Request.Values.Should().BeEmpty();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task GetPersistentAsync_WithoutPersistenceAdapter_ThrowsInvalidOperationException(
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var exception = await Record.ExceptionAsync(() => manager.GetPersistentAsync(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task GetPersistentAsync_WithPersistenceAdapter_ReturnsAttributesFromAdapter(
        [Shared] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        var key = "savedKey";
        var element = JsonSerializer.SerializeToElement(42, AlexaJsonOptions.DefaultOptions);
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement> { [key] = element };
        persistenceAdapter.Configure().GetAttributes(Match.Any<SkillRequest>(), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(adapterData));

        var result = await manager.GetPersistentAsync(TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Values.Should().ContainKey(key);
        persistenceAdapter.Verify().GetAttributes(Match.Any<SkillRequest>(), Match.Any<CancellationToken>()).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task GetPersistentAsync_CalledMultipleTimes_CallsAdapterOnlyOnce(
        [Shared] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement>();
        persistenceAdapter.Configure().GetAttributes(Match.Any<SkillRequest>(), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(adapterData));

        await manager.GetPersistentAsync(TestContext.Current.CancellationToken);
        await manager.GetPersistentAsync(TestContext.Current.CancellationToken);

        persistenceAdapter.Verify().GetAttributes(Match.Any<SkillRequest>(), Match.Any<CancellationToken>()).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task GetPersistentAsync_CalledMultipleTimes_ReturnsSameBagInstance(
        [Shared] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement>();
        persistenceAdapter.Configure().GetAttributes(Match.Any<SkillRequest>(), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(adapterData));

        var first = await manager.GetPersistentAsync(TestContext.Current.CancellationToken);
        var second = await manager.GetPersistentAsync(TestContext.Current.CancellationToken);

        first.Should().BeSameAs(second);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task SavePersistentAttributes_WithoutPersistenceAdapter_ThrowsInvalidOperationException(
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var exception = await Record.ExceptionAsync(() => manager.SavePersistentAttributes(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task SavePersistentAttributes_WithoutLoadingFirst_DoesNotCallAdapter(
        [Shared] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        persistenceAdapter.Verify().SaveAttribute(Match.Any<SkillRequest>(),
            Match.Any<IDictionary<string, JsonElement>>(), Match.Any<CancellationToken>()).Never();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task SavePersistentAttributes_AfterLoadingAttributes_CallsAdapter(
        [Shared] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        IDictionary<string, JsonElement> adapterData = new Dictionary<string, JsonElement>();
        persistenceAdapter.Configure().GetAttributes(Match.Any<SkillRequest>(), Match.Any<CancellationToken>())
            .Returns(Task.FromResult(adapterData));

        await manager.GetPersistentAsync(TestContext.Current.CancellationToken);
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        persistenceAdapter.Verify().SaveAttribute(Match.Any<SkillRequest>(),
            Match.Any<IDictionary<string, JsonElement>>(), Match.Any<CancellationToken>()).Once();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task GetSession_ReturnsSkillRequestSession([Shared] SkillRequest skillRequest,
        SkillRequestFactory factory)
    {
        var manager = new AttributesManager(factory);

        var result = await manager.GetSession(TestContext.Current.CancellationToken);

        result.Should().BeSameAs(skillRequest.Session);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task GetSession_WhenSessionIsNull_ReturnsNull(
        [Shared] SkillRequest skillRequest, SkillRequestFactory factory)
    {
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        var result = await manager.GetSession(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

}