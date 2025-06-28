using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.TestKit.Attributes;
using AutoFixture.Xunit3;

namespace AlexaVoxCraft.MediatR.Tests;

public class DefaultHandlerInputTests : TestBase
{
    [Theory]
    [MediatRAutoData]
    public void Constructor_WithValidInputs_CreatesInstance([Frozen] SkillRequest skillRequest,
        [Frozen] IAttributesManager attributesManager, [Frozen] IResponseBuilder responseBuilder,
        DefaultHandlerInput handlerInput)
    {
        // Assert
        handlerInput.RequestEnvelope.Should().Be(skillRequest);
        handlerInput.AttributesManager.Should().Be(attributesManager);
        handlerInput.ResponseBuilder.Should().Be(responseBuilder);
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException(IAttributesManager attributesManager,
        IResponseBuilder responseBuilder)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(null!, attributesManager, responseBuilder));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullAttributesManager_ThrowsArgumentNullException(
        [Frozen] IResponseBuilder responseBuilder, [Frozen] SkillRequestFactory factory)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(factory, null!, responseBuilder));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullResponseBuilder_ThrowsArgumentNullException([Frozen] SkillRequestFactory factory,
        [Frozen] IAttributesManager attributesManager)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(factory, attributesManager, null!));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
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
    [MediatRAutoData]
    public void RequestEnvelope_ReturnsFactoryResult([Frozen] SkillRequest skillRequest,
        DefaultHandlerInput handlerInput)
    {
        // Act
        var result = handlerInput.RequestEnvelope;

        // Assert
        result.Should().Be(skillRequest);
    }

    [Theory]
    [MediatRAutoData]
    public void AttributesManager_ReturnsProvidedInstance([Frozen] IAttributesManager attributesManager,
        DefaultHandlerInput handlerInput)
    {
        // Act
        var result = handlerInput.AttributesManager;

        // Assert
        result.Should().Be(attributesManager);
    }

    [Theory]
    [MediatRAutoData]
    public void ResponseBuilder_ReturnsProvidedInstance([Frozen] IResponseBuilder responseBuilder,
        DefaultHandlerInput handlerInput)
    {
        // Act
        var result = handlerInput.ResponseBuilder;

        // Assert
        result.Should().Be(responseBuilder);
    }
}