using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;

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
        // Act
        var manager = new AttributesManager(factory);

        // Assert
        manager.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Record.Exception(() => new AttributesManager(null!));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithFactoryReturningNull_ThrowsArgumentNullException(IPersistenceAdapter persistenceAdapter)
    {
        // Arrange
        SkillRequestFactory factory = () => null!;

        // Act & Assert
        var exception = Record.Exception(() => new AttributesManager(factory, persistenceAdapter));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetRequestAttributes_InitiallyEmpty_ReturnsEmptyDictionary(AttributesManager manager)
    {
        // Act
        var result = await manager.GetRequestAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetRequestAttributes_SetsAndReturns_CorrectAttributes(AttributesManager manager,
        IDictionary<string, object> attributes)
    {
        // Act
        await manager.SetRequestAttributes(attributes, TestContext.Current.CancellationToken);
        var result = await manager.GetRequestAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(attributes);
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetSessionAttributes_WithSession_ReturnsSessionAttributes([Frozen] SkillRequest skillRequest,
        SkillRequestFactory factory)
    {
        // Arrange
        var sessionAttributes = new Dictionary<string, object> { ["key1"] = "value1", ["key2"] = 42 };
        skillRequest.Session = new Session { Attributes = sessionAttributes };
        var manager = new AttributesManager(factory);

        // Act
        var result = await manager.GetSessionAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().Equal(sessionAttributes);
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetSessionAttributes_WithNullSession_ThrowsMissingMemberException(
        [Frozen] SkillRequest skillRequest, SkillRequestFactory factory)
    {
        // Arrange
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception =
            await Record.ExceptionAsync(() => manager.GetSessionAttributes(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetSessionAttributes_WithSession_SetsAttributes(AttributesManager manager,
        IDictionary<string, object> newAttributes)
    {
        // Act
        await manager.SetSessionAttributes(newAttributes, TestContext.Current.CancellationToken);
        var result = await manager.GetSessionAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(newAttributes);
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetSessionAttributes_WithNullSession_ThrowsMissingMemberException(
        [Frozen] SkillRequest skillRequest, SkillRequestFactory factory,
        IDictionary<string, object> attributes)
    {
        // Arrange
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            manager.SetSessionAttributes(attributes, TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetPersistentAttributes_WithoutPersistenceAdapter_ThrowsMissingMemberException(
        SkillRequestFactory factory)
    {
        // Arrange
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception =
            await Record.ExceptionAsync(() => manager.GetPersistentAttributes(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetPersistentAttributes_WithPersistenceAdapter_ReturnsAttributesFromAdapter(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager,
        IDictionary<string, object> persistentAttributes)
    {
        // Arrange
        persistenceAdapter.GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>())
            .Returns(persistentAttributes);

        // Act
        var result = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(persistentAttributes);
        await persistenceAdapter.Received(1).GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetPersistentAttributes_CalledMultipleTimes_CallsAdapterOnlyOnce(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager,
        IDictionary<string, object> persistentAttributes)
    {
        // Arrange
        persistenceAdapter.GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>())
            .Returns(persistentAttributes);

        // Act
        var result1 = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);
        var result2 = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        result1.Should().BeSameAs(persistentAttributes);
        result2.Should().BeSameAs(persistentAttributes);
        await persistenceAdapter.Received(1).GetAttributes(Arg.Any<SkillRequest>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetPersistentAttributes_WithoutPersistenceAdapter_ThrowsMissingMemberException(
        SkillRequestFactory factory, IDictionary<string, object> attributes)
    {
        // Arrange
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            manager.SetPersistentAttributes(attributes, TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetPersistentAttributes_WithPersistenceAdapter_SetsAttributes(AttributesManager manager,
        IDictionary<string, object> attributes)
    {
        // Act
        await manager.SetPersistentAttributes(attributes, TestContext.Current.CancellationToken);
        var result = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(attributes);
    }

    [Theory]
    [MediatRAutoData]
    public async Task SavePersistentAttributes_WithoutPersistenceAdapter_ThrowsMissingMemberException(
        SkillRequestFactory factory)
    {
        // Arrange
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception =
            await Record.ExceptionAsync(() => manager.SavePersistentAttributes(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task SavePersistentAttributes_WithoutSettingAttributes_DoesNotCallAdapter(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager)
    {
        // Act
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        await persistenceAdapter.DidNotReceive().SaveAttribute(Arg.Any<SkillRequest>(),
            Arg.Any<IDictionary<string, object>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task SavePersistentAttributes_AfterSettingAttributes_CallsAdapter(
        [Frozen] IPersistenceAdapter persistenceAdapter, AttributesManager manager,
        IDictionary<string, object> attributes)
    {
        // Act
        await manager.SetPersistentAttributes(attributes, TestContext.Current.CancellationToken);
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        await persistenceAdapter.Received(1)
            .SaveAttribute(Arg.Any<SkillRequest>(), attributes, Arg.Any<CancellationToken>());
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetSession_WithSession_ReturnsSessionWithUpdatedAttributes([Frozen] SkillRequest skillRequest,
        SkillRequestFactory factory,
        IDictionary<string, object> newAttributes)
    {
        // Arrange
        var originalAttributes = new Dictionary<string, object> { ["old"] = "value" };
        skillRequest.Session = new Session { Attributes = originalAttributes };
        var manager = new AttributesManager(factory);

        // Act
        await manager.SetSessionAttributes(newAttributes, TestContext.Current.CancellationToken);
        var result = await manager.GetSession(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Attributes.Should().Equal(newAttributes);
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetSession_WithNullSession_ThrowsMissingMemberException([Frozen] SkillRequest skillRequest,
        SkillRequestFactory factory)
    {
        // Arrange
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => manager.GetSession(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetSessionState_WhenNotSet_ReturnsNewInstance(AttributesManager manager)
    {
        // Act
        var result = await manager.GetSessionState<TestSkillState>(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CurrentIntent.Should().Be(string.Empty);
        result.TurnCount.Should().Be(0);
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetSessionState_ThenGetSessionState_RoundTripsState(AttributesManager manager)
    {
        // Arrange
        var state = new TestSkillState { CurrentIntent = "PlayMusic", TurnCount = 3 };

        // Act
        await manager.SetSessionState(state, TestContext.Current.CancellationToken);
        var result = await manager.GetSessionState<TestSkillState>(TestContext.Current.CancellationToken);

        // Assert
        result.CurrentIntent.Should().Be("PlayMusic");
        result.TurnCount.Should().Be(3);
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetSessionState_StoresUnderFullTypeName(AttributesManager manager)
    {
        // Arrange
        var state = new TestSkillState { TurnCount = 7 };

        // Act
        await manager.SetSessionState(state, TestContext.Current.CancellationToken);
        var attributes = await manager.GetSessionAttributes(TestContext.Current.CancellationToken);

        // Assert
        attributes.Should().ContainKey(typeof(TestSkillState).FullName!);
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetSessionState_WithNullSession_ThrowsMissingMemberException([Frozen] SkillRequest skillRequest,
        SkillRequestFactory factory)
    {
        // Arrange
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            manager.GetSessionState<TestSkillState>(TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task SetSessionState_WithNullSession_ThrowsMissingMemberException([Frozen] SkillRequest skillRequest,
        SkillRequestFactory factory)
    {
        // Arrange
        skillRequest.Session = null!;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            manager.SetSessionState(new TestSkillState(), TestContext.Current.CancellationToken));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }
}