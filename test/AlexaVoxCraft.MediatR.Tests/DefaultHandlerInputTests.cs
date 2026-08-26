using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Tests;

public class DefaultHandlerInputTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithValidInputs_CreatesInstance([Shared] SkillRequest skillRequest,
        [Shared] IAttributesManager attributesManager, [Shared] IResponseBuilder responseBuilder,
        DefaultHandlerInput handlerInput)
    {
        // Assert
        handlerInput.RequestEnvelope.Should().Be(skillRequest);
        handlerInput.AttributesManager.Should().Be(attributesManager);
        handlerInput.ResponseBuilder.Should().Be(responseBuilder);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException(IAttributesManager attributesManager,
        IResponseBuilder responseBuilder)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(null!, attributesManager, responseBuilder));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullAttributesManager_ThrowsArgumentNullException(
        [Shared] IResponseBuilder responseBuilder, [Shared] SkillRequestFactory factory)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(factory, null!, responseBuilder));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullResponseBuilder_ThrowsArgumentNullException([Shared] SkillRequestFactory factory,
        [Shared] IAttributesManager attributesManager)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultHandlerInput(factory, attributesManager, null!));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
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
    [Compose<MediatRTestProfile>]
    public void RequestEnvelope_ReturnsFactoryResult([Shared] SkillRequest skillRequest,
        DefaultHandlerInput handlerInput)
    {
        // Act
        var result = handlerInput.RequestEnvelope;

        // Assert
        result.Should().Be(skillRequest);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void AttributesManager_ReturnsProvidedInstance([Shared] IAttributesManager attributesManager,
        DefaultHandlerInput handlerInput)
    {
        // Act
        var result = handlerInput.AttributesManager;

        // Assert
        result.Should().Be(attributesManager);
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void ResponseBuilder_ReturnsProvidedInstance([Shared] IResponseBuilder responseBuilder,
        DefaultHandlerInput handlerInput)
    {
        // Act
        var result = handlerInput.ResponseBuilder;

        // Assert
        result.Should().Be(responseBuilder);
    }
}