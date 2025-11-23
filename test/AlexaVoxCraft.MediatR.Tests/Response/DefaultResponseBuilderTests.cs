using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Ssml;
using AlexaVoxCraft.TestKit.Attributes;
using AutoFixture.Xunit3;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests.Response;

public class DefaultResponseBuilderTests : TestBase
{
    [Theory]
    [MediatRAutoData]
    public void Constructor_WithValidAttributesManager_CreatesInstance(
        IAttributesManager attributesManager,
        IOptions<SkillServiceConfiguration> skillServiceConfiguration)
    {
        // Act
        var builder = new DefaultResponseBuilder(attributesManager, skillServiceConfiguration);

        // Assert
        builder.Should().NotBeNull();
    }

    [Theory]
    [MediatRAutoData]
    public void Constructor_WithNullAttributesManager_ThrowsArgumentNullException(
        IOptions<SkillServiceConfiguration> skillServiceConfiguration)
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultResponseBuilder(null!, skillServiceConfiguration));

        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Speak_WithPlainText_SetsOutputSpeech(DefaultResponseBuilder builder, string speechText)
    {
        // Act
        builder.Speak(speechText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.OutputSpeech.Should().NotBeNull();
        var ssmlSpeech = response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain(speechText.Trim());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Speak_WithSsmlElements_SetsOutputSpeech(DefaultResponseBuilder builder)
    {
        // Arrange
        var plainText = new PlainText("Hello");
        var audio = new Audio("https://example.com/audio.mp3");

        // Act
        builder.Speak(plainText, audio);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.OutputSpeech.Should().NotBeNull();
        var ssmlSpeech = response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain("Hello");
        ssmlSpeech.Ssml.Should().Contain("https://example.com/audio.mp3");
    }

    [Theory]
    [MediatRAutoData]
    public async Task SpeakAudio_WithUrl_SetsAudioOutputSpeech(DefaultResponseBuilder builder, string audioUrl)
    {
        // Act
        builder.SpeakAudio(audioUrl);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.OutputSpeech.Should().NotBeNull();
        var ssmlSpeech = response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain(audioUrl.Trim());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Reprompt_WithPlainText_SetsReprompt(DefaultResponseBuilder builder, string repromptText)
    {
        // Act
        builder.Reprompt(repromptText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Reprompt.Should().NotBeNull();
        var ssmlSpeech = response.Response.Reprompt.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain(repromptText.Trim());
        response.Response.ShouldEndSession.Should().BeFalse();
    }

    [Theory]
    [MediatRAutoData]
    public async Task Reprompt_WithSsmlElements_SetsReprompt(DefaultResponseBuilder builder)
    {
        // Arrange
        var plainText = new PlainText("Please respond");

        // Act
        builder.Reprompt(plainText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Reprompt.Should().NotBeNull();
        response.Response.Reprompt.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>();
        response.Response.ShouldEndSession.Should().BeFalse();
    }

    [Theory]
    [MediatRAutoData]
    public async Task WithSimpleCard_SetsSimpleCard(DefaultResponseBuilder builder, string title, string content)
    {
        // Act
        builder.WithSimpleCard(title, content);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        var card = response.Response.Card.Should().BeOfType<SimpleCard>().Subject;
        card.Title.Should().Be(title);
        card.Content.Should().Be(content);
    }

    [Theory]
    [MediatRAutoData]
    public async Task WithStandardCard_WithoutImages_SetsStandardCard(DefaultResponseBuilder builder, string title,
        string content)
    {
        // Act
        builder.WithStandardCard(title, content);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        var card = response.Response.Card.Should().BeOfType<StandardCard>().Subject;
        card.Title.Should().Be(title);
        card.Content.Should().Be(content);
        card.Image.Should().BeNull();
    }

    [Theory]
    [MediatRAutoData]
    public async Task WithStandardCard_WithImages_SetsStandardCardWithImages(DefaultResponseBuilder builder,
        string title, string content, string smallImageUrl, string largeImageUrl)
    {
        // Act
        builder.WithStandardCard(title, content, smallImageUrl, largeImageUrl);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        var card = response.Response.Card.Should().BeOfType<StandardCard>().Subject;
        card.Title.Should().Be(title);
        card.Content.Should().Be(content);
        card.Image.Should().NotBeNull();
        card.Image.SmallImageUrl.Should().Be(smallImageUrl);
        card.Image.LargeImageUrl.Should().Be(largeImageUrl);
    }

    [Theory]
    [MediatRAutoData]
    public async Task WithLinkAccountCard_SetsLinkAccountCard(DefaultResponseBuilder builder)
    {
        // Act
        builder.WithLinkAccountCard();
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        response.Response.Card.Should().BeOfType<LinkAccountCard>();
    }

    [Theory]
    [MediatRAutoData]
    public async Task WithAskForPermissionsConsentCard_SetsPermissionsCard(DefaultResponseBuilder builder,
        List<string> permissions)
    {
        // Act
        builder.WithAskForPermissionsConsentCard(permissions);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        var card = response.Response.Card.Should().BeOfType<AskForPermissionsConsentCard>().Subject;
        card.Permissions.Should().Equal(permissions);
    }

    [Theory]
    [MediatRAutoData]
    public async Task AddAudioPlayerPlayDirective_AddsPlayDirective(DefaultResponseBuilder builder, string url,
        string token, int offset)
    {
        // Act
        builder.AddAudioPlayerPlayDirective(PlayBehavior.ReplaceAll, url, token, offset,
            cancellationToken: TestContext.Current.CancellationToken);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Directives.Should().NotBeNull();
        response.Response.Directives.Should().HaveCount(1);
        var directive = response.Response.Directives[0].Should().BeOfType<AudioPlayerPlayDirective>().Subject;
        directive.PlayBehavior.Should().Be(PlayBehavior.ReplaceAll);
        directive.AudioItem.Stream.Url.Should().Be(url);
        directive.AudioItem.Stream.Token.Should().Be(token);
        directive.AudioItem.Stream.OffsetInMilliseconds.Should().Be(offset);
    }

    [Theory]
    [MediatRAutoData]
    public async Task AddAudioPlayerPlayDirective_WithExpectedPreviousToken_SetsExpectedToken(
        DefaultResponseBuilder builder, string url, string token, string expectedToken, int offset)
    {
        // Act
        builder.AddAudioPlayerPlayDirective(PlayBehavior.Enqueue, url, token, offset, expectedToken,
            cancellationToken: TestContext.Current.CancellationToken);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        var directive = response.Response.Directives[0].Should().BeOfType<AudioPlayerPlayDirective>().Subject;
        directive.AudioItem.Stream.ExpectedPreviousToken.Should().Be(expectedToken);
    }

    [Theory]
    [MediatRAutoData]
    public async Task WithShouldEndSession_True_SetsEndSession(DefaultResponseBuilder builder)
    {
        // Act
        builder.WithShouldEndSession(true);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.ShouldEndSession.Should().BeTrue();
    }

    [Theory]
    [MediatRAutoData]
    public async Task WithShouldEndSession_False_SetsEndSession(DefaultResponseBuilder builder)
    {
        // Act
        builder.WithShouldEndSession(false);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.ShouldEndSession.Should().BeFalse();
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetResponse_CallsGetSessionAttributesOnAttributesManager(
        [Frozen] IAttributesManager attributesManager, DefaultResponseBuilder builder,
        Dictionary<string, object> sessionAttributes)
    {
        // Arrange
        attributesManager.GetSessionAttributes(Arg.Any<CancellationToken>()).Returns(sessionAttributes);

        // Act
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        await attributesManager.Received(1).GetSessionAttributes(Arg.Any<CancellationToken>());
        response.SessionAttributes.Should().Equal(sessionAttributes);
    }

    [Theory]
    [MediatRAutoData]
    public async Task GetResponse_ReturnsCompleteSkillResponse([Frozen] IAttributesManager attributesManager,
        DefaultResponseBuilder builder, string speechText)
    {
        // Arrange
        var session = new Session { Attributes = new Dictionary<string, object>() };
        attributesManager.GetSession(Arg.Any<CancellationToken>()).Returns(session);

        // Act
        builder.Speak(speechText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<SkillResponse>();
        response.Response.Should().NotBeNull();
        response.Version.Should().Be("1.0");
    }

    [Theory]
    [MediatRAutoData]
    public void FluentInterface_AllMethodsReturnBuilder(DefaultResponseBuilder builder, string text)
    {
        // Act & Assert
        builder.Speak(text).Should().Be(builder);
        builder.Reprompt(text).Should().Be(builder);
        builder.WithSimpleCard("title", "content").Should().Be(builder);
        builder.WithShouldEndSession(true).Should().Be(builder);
    }

    [Theory]
    [MediatRAutoData]
    public async Task Speak_WithDefaultVoice_WrapsOutputInVoiceElement(
        IAttributesManager attributesManager,
        IOptions<SkillServiceConfiguration> voicedConfiguration,
        string speechText)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager, voicedConfiguration);

        // Act
        builder.Speak(speechText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.OutputSpeech.Should().NotBeNull();
        var ssmlSpeech = response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain($"<voice name=\"{PollyVoices.EnglishUS.Matthew}\">");
        ssmlSpeech.Ssml.Should().Contain(speechText.Trim());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Speak_WithDefaultVoice_AndSsmlElements_WrapsInVoiceElement(
        IAttributesManager attributesManager,
        IOptions<SkillServiceConfiguration> voicedConfiguration)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager, voicedConfiguration);
        var plainText = new PlainText("Hello world");

        // Act
        builder.Speak(plainText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        var ssmlSpeech = response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain($"<voice name=\"{PollyVoices.EnglishUS.Matthew}\">");
        ssmlSpeech.Ssml.Should().Contain("Hello world");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Reprompt_WithDefaultVoice_WrapsOutputInVoiceElement(
        IAttributesManager attributesManager,
        IOptions<SkillServiceConfiguration> voicedConfiguration,
        string repromptText)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager, voicedConfiguration);

        // Act
        builder.Reprompt(repromptText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Reprompt.Should().NotBeNull();
        var ssmlSpeech = response.Response.Reprompt.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain($"<voice name=\"{PollyVoices.EnglishUS.Matthew}\">");
        ssmlSpeech.Ssml.Should().Contain(repromptText.Trim());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Reprompt_WithDefaultVoice_AndSsmlElements_WrapsInVoiceElement(
        IAttributesManager attributesManager,
        IOptions<SkillServiceConfiguration> voicedConfiguration)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager, voicedConfiguration);
        var plainText = new PlainText("Please respond");

        // Act
        builder.Reprompt(plainText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        var ssmlSpeech = response.Response.Reprompt.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain($"<voice name=\"{PollyVoices.EnglishUS.Matthew}\">");
        ssmlSpeech.Ssml.Should().Contain("Please respond");
    }

    [Theory]
    [MediatRAutoData]
    public async Task Speak_WithoutDefaultVoice_DoesNotWrapInVoiceElement(
        DefaultResponseBuilder builder,
        string speechText)
    {
        // Act
        builder.Speak(speechText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        var ssmlSpeech = response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().NotContain("<voice");
        ssmlSpeech.Ssml.Should().Contain(speechText.Trim());
    }

    [Theory]
    [MediatRAutoData]
    public async Task Reprompt_WithoutDefaultVoice_DoesNotWrapInVoiceElement(
        DefaultResponseBuilder builder,
        string repromptText)
    {
        // Act
        builder.Reprompt(repromptText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        var ssmlSpeech = response.Response.Reprompt.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().NotContain("<voice");
        ssmlSpeech.Ssml.Should().Contain(repromptText.Trim());
    }

    [Theory]
    [MediatRAutoData]
    public async Task SpeakAudio_WithDefaultVoice_WrapsAudioInVoiceElement(
        IAttributesManager attributesManager,
        IOptions<SkillServiceConfiguration> voicedConfiguration,
        string audioUrl)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager, voicedConfiguration);

        // Act
        builder.SpeakAudio(audioUrl);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        var ssmlSpeech = response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>().Subject;
        ssmlSpeech.Ssml.Should().Contain($"<voice name=\"{PollyVoices.EnglishUS.Matthew}\">");
        ssmlSpeech.Ssml.Should().Contain(audioUrl.Trim());
    }
}