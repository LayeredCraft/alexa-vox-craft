using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;
using AutoFixture.Xunit3;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests;

public class DefaultHandlerInputTests : TestBase
{
    [Theory]
    [AutoData]
    public void Constructor_WithValidInputs_CreatesInstance(SkillRequest skillRequest,
        IAttributesManager attributesManager, IResponseBuilder responseBuilder, [Frozen] SkillRequestFactory factory)
    {
        // Arrange
        factory.Invoke().Returns(skillRequest);

        // Act
        var handlerInput = new DefaultHandlerInput(factory, attributesManager, responseBuilder);

        // Assert
        handlerInput.RequestEnvelope.Should().Be(skillRequest);
        handlerInput.AttributesManager.Should().Be(attributesManager);
        handlerInput.ResponseBuilder.Should().Be(responseBuilder);
    }

    [Theory]
    [AutoData]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException(IAttributesManager attributesManager,
        IResponseBuilder responseBuilder)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(null!, attributesManager, responseBuilder));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public void Constructor_WithNullAttributesManager_ThrowsArgumentNullException(SkillRequest skillRequest,
        IResponseBuilder responseBuilder)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;

        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(factory, null!, responseBuilder));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public void Constructor_WithNullResponseBuilder_ThrowsArgumentNullException(SkillRequest skillRequest,
        IAttributesManager attributesManager)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;

        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(factory, attributesManager, null!));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public void Constructor_WithFactoryReturningNull_ThrowsArgumentNullException(IAttributesManager attributesManager,
        IResponseBuilder responseBuilder)
    {
        // Arrange
        SkillRequestFactory factory = () => null!;

        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(factory, attributesManager, responseBuilder));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public void RequestEnvelope_ReturnsFactoryResult(SkillRequest skillRequest, IAttributesManager attributesManager,
        IResponseBuilder responseBuilder)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var handlerInput = new DefaultHandlerInput(factory, attributesManager, responseBuilder);

        // Act
        var result = handlerInput.RequestEnvelope;

        // Assert
        result.Should().Be(skillRequest);
    }

    [Theory]
    [AutoData]
    public void AttributesManager_ReturnsProvidedInstance(SkillRequest skillRequest,
        IAttributesManager attributesManager, IResponseBuilder responseBuilder)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var handlerInput = new DefaultHandlerInput(factory, attributesManager, responseBuilder);

        // Act
        var result = handlerInput.AttributesManager;

        // Assert
        result.Should().Be(attributesManager);
    }

    [Theory]
    [AutoData]
    public void ResponseBuilder_ReturnsProvidedInstance(SkillRequest skillRequest, IAttributesManager attributesManager,
        IResponseBuilder responseBuilder)
    {
        // Arrange
        SkillRequestFactory factory = () => skillRequest;
        var handlerInput = new DefaultHandlerInput(factory, attributesManager, responseBuilder);

        // Act
        var result = handlerInput.ResponseBuilder;

        // Assert
        result.Should().Be(responseBuilder);
    }
}