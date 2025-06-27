using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.Model.Request;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests.Attributes;

public class AttributesManagerTests : TestBase
{
    [Theory]
    [AutoData]
    public void Constructor_WithValidFactory_CreatesInstance(SkillRequest skillRequest)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;

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
    [AutoData]
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
    [AutoData]
    public async Task GetRequestAttributes_InitiallyEmpty_ReturnsEmptyDictionary(SkillRequest skillRequest)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act
        var result = await manager.GetRequestAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [AutoData]
    public async Task SetRequestAttributes_SetsAndReturns_CorrectAttributes(SkillRequest skillRequest, IDictionary<string, object> attributes)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act
        await manager.SetRequestAttributes(attributes, TestContext.Current.CancellationToken);
        var result = await manager.GetRequestAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(attributes);
    }

    [Theory]
    [AutoData]
    public async Task GetSessionAttributes_WithSession_ReturnsSessionAttributes(SkillRequest skillRequest)
    {
        // Arrange
        var sessionAttributes = new Dictionary<string, object> { ["key1"] = "value1", ["key2"] = 42 };
        skillRequest.Session = new Session { Attributes = sessionAttributes };
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act
        var result = await manager.GetSessionAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().Equal(sessionAttributes);
    }

    [Theory]
    [AutoData]
    public async Task GetSessionAttributes_WithNullSession_ThrowsMissingMemberException(SkillRequest skillRequest)
    {
        // Arrange
        skillRequest.Session = null!;
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => manager.GetSessionAttributes(TestContext.Current.CancellationToken));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [AutoData]
    public async Task SetSessionAttributes_WithSession_SetsAttributes(SkillRequest skillRequest, IDictionary<string, object> newAttributes)
    {
        // Arrange
        skillRequest.Session = new Session { Attributes = new Dictionary<string, object>() };
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act
        await manager.SetSessionAttributes(newAttributes, TestContext.Current.CancellationToken);
        var result = await manager.GetSessionAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(newAttributes);
    }

    [Theory]
    [AutoData]
    public async Task SetSessionAttributes_WithNullSession_ThrowsMissingMemberException(SkillRequest skillRequest, IDictionary<string, object> attributes)
    {
        // Arrange
        skillRequest.Session = null!;
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => manager.SetSessionAttributes(attributes, TestContext.Current.CancellationToken));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [AutoData]
    public async Task GetPersistentAttributes_WithoutPersistenceAdapter_ThrowsMissingMemberException(SkillRequest skillRequest)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => manager.GetPersistentAttributes(TestContext.Current.CancellationToken));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [AutoData]
    public async Task GetPersistentAttributes_WithPersistenceAdapter_ReturnsAttributesFromAdapter(SkillRequest skillRequest, IDictionary<string, object> persistentAttributes)
    {
        // Arrange
        var persistenceAdapter = CreateSubstitute<IPersistenceAdapter>();
        persistenceAdapter.GetAttributes(skillRequest, Arg.Any<CancellationToken>()).Returns(persistentAttributes);
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory, persistenceAdapter);

        // Act
        var result = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(persistentAttributes);
        await persistenceAdapter.Received(1).GetAttributes(skillRequest, Arg.Any<CancellationToken>());
    }

    [Theory]
    [AutoData]
    public async Task GetPersistentAttributes_CalledMultipleTimes_CallsAdapterOnlyOnce(SkillRequest skillRequest, IDictionary<string, object> persistentAttributes)
    {
        // Arrange
        var persistenceAdapter = CreateSubstitute<IPersistenceAdapter>();
        persistenceAdapter.GetAttributes(skillRequest, Arg.Any<CancellationToken>()).Returns(persistentAttributes);
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory, persistenceAdapter);

        // Act
        var result1 = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);
        var result2 = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        result1.Should().BeSameAs(persistentAttributes);
        result2.Should().BeSameAs(persistentAttributes);
        await persistenceAdapter.Received(1).GetAttributes(skillRequest, Arg.Any<CancellationToken>());
    }

    [Theory]
    [AutoData]
    public async Task SetPersistentAttributes_WithoutPersistenceAdapter_ThrowsMissingMemberException(SkillRequest skillRequest, IDictionary<string, object> attributes)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => manager.SetPersistentAttributes(attributes, TestContext.Current.CancellationToken));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [AutoData]
    public async Task SetPersistentAttributes_WithPersistenceAdapter_SetsAttributes(SkillRequest skillRequest, IDictionary<string, object> attributes)
    {
        // Arrange
        var persistenceAdapter = CreateSubstitute<IPersistenceAdapter>();
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory, persistenceAdapter);

        // Act
        await manager.SetPersistentAttributes(attributes, TestContext.Current.CancellationToken);
        var result = await manager.GetPersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeSameAs(attributes);
    }

    [Theory]
    [AutoData]
    public async Task SavePersistentAttributes_WithoutPersistenceAdapter_ThrowsMissingMemberException(SkillRequest skillRequest)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => manager.SavePersistentAttributes(TestContext.Current.CancellationToken));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }

    [Theory]
    [AutoData]
    public async Task SavePersistentAttributes_WithoutSettingAttributes_DoesNotCallAdapter(SkillRequest skillRequest)
    {
        // Arrange
        var persistenceAdapter = CreateSubstitute<IPersistenceAdapter>();
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory, persistenceAdapter);

        // Act
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        await persistenceAdapter.DidNotReceive().SaveAttribute(Arg.Any<SkillRequest>(), Arg.Any<IDictionary<string, object>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [AutoData]
    public async Task SavePersistentAttributes_AfterSettingAttributes_CallsAdapter(SkillRequest skillRequest, IDictionary<string, object> attributes)
    {
        // Arrange
        var persistenceAdapter = CreateSubstitute<IPersistenceAdapter>();
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory, persistenceAdapter);

        // Act
        await manager.SetPersistentAttributes(attributes, TestContext.Current.CancellationToken);
        await manager.SavePersistentAttributes(TestContext.Current.CancellationToken);

        // Assert
        await persistenceAdapter.Received(1).SaveAttribute(skillRequest, attributes, Arg.Any<CancellationToken>());
    }

    [Theory]
    [AutoData]
    public async Task GetSession_WithSession_ReturnsSessionWithUpdatedAttributes(SkillRequest skillRequest, IDictionary<string, object> newAttributes)
    {
        // Arrange
        var originalAttributes = new Dictionary<string, object> { ["old"] = "value" };
        skillRequest.Session = new Session { Attributes = originalAttributes };
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act
        await manager.SetSessionAttributes(newAttributes, TestContext.Current.CancellationToken);
        var result = await manager.GetSession(TestContext.Current.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Attributes.Should().Equal(newAttributes);
    }

    [Theory]
    [AutoData]
    public async Task GetSession_WithNullSession_ThrowsMissingMemberException(SkillRequest skillRequest)
    {
        // Arrange
        skillRequest.Session = null!;
        SkillRequestFactory factory = () => skillRequest;
        var manager = new AttributesManager(factory);

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => manager.GetSession(TestContext.Current.CancellationToken));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<MissingMemberException>();
    }
}