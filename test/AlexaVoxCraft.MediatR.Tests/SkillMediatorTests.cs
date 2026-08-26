using Compono.XunitV3;
using AlexaVoxCraft.MediatR.Tests.TestKit;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.Model.Request;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR.Tests;

public class SkillMediatorTests : TestBase
{
    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException(
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        var exception = Record.Exception(() => new SkillMediator(null!, validConfiguration));

        exception.Should().BeOfType<ArgumentNullException>().Subject.ParamName.Should().Be("serviceProvider");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullConfiguration_ThrowsNullReferenceException(
        IServiceProvider serviceProvider)
    {
        var exception = Record.Exception(() => new SkillMediator(serviceProvider, null!));

        exception.Should().BeOfType<NullReferenceException>();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithNullConfigurationValue_ThrowsArgumentNullException(
        IServiceProvider serviceProvider)
    {
        // Arrange - an IOptions<T> whose Value is itself null
        var nullConfigurationValue = TestHelper.SkillOptionsWithNullValue();

        var exception = Record.Exception(() => new SkillMediator(serviceProvider, nullConfigurationValue));

        exception.Should().BeOfType<ArgumentNullException>().Subject.ParamName.Should().Be("serviceConfiguration");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public void Constructor_WithValidParameters_CreatesInstance(
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        var mediator = new SkillMediator(serviceProvider, validConfiguration);

        mediator.Should().NotBeNull();
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithNullSkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider)
    {
        var nullConfiguration = TestHelper.SkillOptions(skillId: null);
        var mediator = new SkillMediator(serviceProvider, nullConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithEmptySkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider)
    {
        var emptyConfiguration = TestHelper.SkillOptions(skillId: string.Empty);
        var mediator = new SkillMediator(serviceProvider, emptyConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithWhitespaceSkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider)
    {
        var whitespaceConfiguration = TestHelper.SkillOptions(skillId: "   ");
        var mediator = new SkillMediator(serviceProvider, whitespaceConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithMismatchedSkillId_ThrowsArgumentException(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> invalidConfiguration)
    {
        var mediator = new SkillMediator(serviceProvider, invalidConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        exception.Should().BeOfType<ArgumentException>().Subject.Message.Should().Be("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_WithMatchingSkillId_PassesSkillIdValidation(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        skillRequest.Context.System.Application.ApplicationId = validConfiguration.Value.SkillId!;

        var mediator = new SkillMediator(serviceProvider, validConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        // Should not be the skill ID verification exception
        exception.Should().NotBeOfType<ArgumentException>();
        exception.Message.Should().NotBe("Skill ID verification failed!");
    }

    [Theory]
    [Compose<MediatRTestProfile>]
    public async Task Send_CreatesHandlerWrapper_ThrowsInvalidOperationExceptionForUnsupportedType(
        SkillRequest skillRequest,
        IServiceProvider serviceProvider,
        IOptions<SkillServiceConfiguration> validConfiguration)
    {
        skillRequest.Context.System.Application.ApplicationId = validConfiguration.Value.SkillId!;

        var mediator = new SkillMediator(serviceProvider, validConfiguration);

        var exception = await Record.ExceptionAsync(() => mediator.Send(skillRequest, CancellationToken));

        // The exception thrown when trying to create the wrapper for unsupported request types
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Should().BeOfType<InvalidOperationException>().Subject.Message.Should().Contain("Handler was not found for request of type");
    }
}