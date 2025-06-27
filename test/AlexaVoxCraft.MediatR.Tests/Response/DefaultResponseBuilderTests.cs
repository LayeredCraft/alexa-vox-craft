using AwesomeAssertions;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Response;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Ssml;
using NSubstitute;

namespace AlexaVoxCraft.MediatR.Tests.Response;

public class DefaultResponseBuilderTests : TestBase
{
    [Theory]
    [AutoData]
    public void Constructor_WithValidAttributesManager_CreatesInstance(IAttributesManager attributesManager)
    {
        // Act
        var builder = new DefaultResponseBuilder(attributesManager);

        // Assert
        builder.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullAttributesManager_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Record.Exception(() => new DefaultResponseBuilder(null!));
        
        exception.Should().NotBeNull();
        exception.Should().BeOfType<ArgumentNullException>();
    }

    [Theory]
    [AutoData]
    public async Task Speak_WithPlainText_SetsOutputSpeech(IAttributesManager attributesManager, string speechText)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.Speak(speechText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.OutputSpeech.Should().NotBeNull();
        response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>();
        var ssmlSpeech = (SsmlOutputSpeech)response.Response.OutputSpeech;
        ssmlSpeech.Ssml.Should().Contain(speechText.Trim());
    }

    [Theory]
    [AutoData]
    public async Task Speak_WithSsmlElements_SetsOutputSpeech(IAttributesManager attributesManager)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);
        var plainText = new PlainText("Hello");
        var audio = new Audio("https://example.com/audio.mp3");

        // Act
        builder.Speak(plainText, audio);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.OutputSpeech.Should().NotBeNull();
        response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>();
        var ssmlSpeech = (SsmlOutputSpeech)response.Response.OutputSpeech;
        ssmlSpeech.Ssml.Should().Contain("Hello");
        ssmlSpeech.Ssml.Should().Contain("https://example.com/audio.mp3");
    }

    [Theory]
    [AutoData]
    public async Task SpeakAudio_WithUrl_SetsAudioOutputSpeech(IAttributesManager attributesManager, string audioUrl)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.SpeakAudio(audioUrl);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.OutputSpeech.Should().NotBeNull();
        response.Response.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>();
        var ssmlSpeech = (SsmlOutputSpeech)response.Response.OutputSpeech;
        ssmlSpeech.Ssml.Should().Contain(audioUrl.Trim());
    }

    [Theory]
    [AutoData]
    public async Task Reprompt_WithPlainText_SetsReprompt(IAttributesManager attributesManager, string repromptText)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.Reprompt(repromptText);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Reprompt.Should().NotBeNull();
        response.Response.Reprompt.OutputSpeech.Should().BeOfType<SsmlOutputSpeech>();
        var ssmlSpeech = (SsmlOutputSpeech)response.Response.Reprompt.OutputSpeech;
        ssmlSpeech.Ssml.Should().Contain(repromptText.Trim());
        response.Response.ShouldEndSession.Should().BeFalse();
    }

    [Theory]
    [AutoData]
    public async Task Reprompt_WithSsmlElements_SetsReprompt(IAttributesManager attributesManager)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);
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
    [AutoData]
    public async Task WithSimpleCard_SetsSimpleCard(IAttributesManager attributesManager, string title, string content)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.WithSimpleCard(title, content);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        response.Response.Card.Should().BeOfType<SimpleCard>();
        var card = (SimpleCard)response.Response.Card;
        card.Title.Should().Be(title);
        card.Content.Should().Be(content);
    }

    [Theory]
    [AutoData]
    public async Task WithStandardCard_WithoutImages_SetsStandardCard(IAttributesManager attributesManager, string title, string content)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.WithStandardCard(title, content);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        response.Response.Card.Should().BeOfType<StandardCard>();
        var card = (StandardCard)response.Response.Card;
        card.Title.Should().Be(title);
        card.Content.Should().Be(content);
        card.Image.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task WithStandardCard_WithImages_SetsStandardCardWithImages(IAttributesManager attributesManager, string title, string content, string smallImageUrl, string largeImageUrl)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.WithStandardCard(title, content, smallImageUrl, largeImageUrl);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        response.Response.Card.Should().BeOfType<StandardCard>();
        var card = (StandardCard)response.Response.Card;
        card.Title.Should().Be(title);
        card.Content.Should().Be(content);
        card.Image.Should().NotBeNull();
        card.Image.SmallImageUrl.Should().Be(smallImageUrl);
        card.Image.LargeImageUrl.Should().Be(largeImageUrl);
    }

    [Theory]
    [AutoData]
    public async Task WithLinkAccountCard_SetsLinkAccountCard(IAttributesManager attributesManager)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.WithLinkAccountCard();
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        response.Response.Card.Should().BeOfType<LinkAccountCard>();
    }

    [Theory]
    [AutoData]
    public async Task WithAskForPermissionsConsentCard_SetsPermissionsCard(IAttributesManager attributesManager, List<string> permissions)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.WithAskForPermissionsConsentCard(permissions);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Card.Should().NotBeNull();
        response.Response.Card.Should().BeOfType<AskForPermissionsConsentCard>();
        var card = (AskForPermissionsConsentCard)response.Response.Card;
        card.Permissions.Should().Equal(permissions);
    }

    [Theory]
    [AutoData]
    public async Task AddAudioPlayerPlayDirective_AddsPlayDirective(IAttributesManager attributesManager, string url, string token, int offset)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.AddAudioPlayerPlayDirective(PlayBehavior.ReplaceAll, url, token, offset, cancellationToken: TestContext.Current.CancellationToken);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.Directives.Should().NotBeNull();
        response.Response.Directives.Should().HaveCount(1);
        response.Response.Directives[0].Should().BeOfType<AudioPlayerPlayDirective>();
        var directive = (AudioPlayerPlayDirective)response.Response.Directives[0];
        directive.PlayBehavior.Should().Be(PlayBehavior.ReplaceAll);
        directive.AudioItem.Stream.Url.Should().Be(url);
        directive.AudioItem.Stream.Token.Should().Be(token);
        directive.AudioItem.Stream.OffsetInMilliseconds.Should().Be(offset);
    }

    [Theory]
    [AutoData]
    public async Task AddAudioPlayerPlayDirective_WithExpectedPreviousToken_SetsExpectedToken(IAttributesManager attributesManager, string url, string token, string expectedToken, int offset)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.AddAudioPlayerPlayDirective(PlayBehavior.Enqueue, url, token, offset, expectedToken, cancellationToken: TestContext.Current.CancellationToken);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        var directive = (AudioPlayerPlayDirective)response.Response.Directives[0];
        directive.AudioItem.Stream.ExpectedPreviousToken.Should().Be(expectedToken);
    }

    [Theory]
    [AutoData]
    public async Task WithShouldEndSession_True_SetsEndSession(IAttributesManager attributesManager)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.WithShouldEndSession(true);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.ShouldEndSession.Should().BeTrue();
    }

    [Theory]
    [AutoData]
    public async Task WithShouldEndSession_False_SetsEndSession(IAttributesManager attributesManager)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        builder.WithShouldEndSession(false);
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        response.Response.ShouldEndSession.Should().BeFalse();
    }

    [Theory]
    [AutoData]
    public async Task GetResponse_CallsGetSessionAttributesOnAttributesManager(Dictionary<string, object> sessionAttributes)
    {
        // Arrange
        var attributesManager = CreateSubstitute<IAttributesManager>();
        attributesManager.GetSessionAttributes(Arg.Any<CancellationToken>()).Returns(sessionAttributes);
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act
        var response = await builder.GetResponse(TestContext.Current.CancellationToken);

        // Assert
        await attributesManager.Received(1).GetSessionAttributes(Arg.Any<CancellationToken>());
        response.SessionAttributes.Should().Equal(sessionAttributes);
    }

    [Theory]
    [AutoData]
    public async Task GetResponse_ReturnsCompleteSkillResponse(IAttributesManager attributesManager, string speechText)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);
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
    [AutoData]
    public void FluentInterface_AllMethodsReturnBuilder(IAttributesManager attributesManager, string text)
    {
        // Arrange
        var builder = new DefaultResponseBuilder(attributesManager);

        // Act & Assert
        builder.Speak(text).Should().Be(builder);
        builder.Reprompt(text).Should().Be(builder);
        builder.WithSimpleCard("title", "content").Should().Be(builder);
        builder.WithShouldEndSession(true).Should().Be(builder);
    }
}