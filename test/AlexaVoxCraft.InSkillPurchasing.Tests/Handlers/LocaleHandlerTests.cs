using System.Net;
using AlexaVoxCraft.Http.TestKit.Extensions;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Handlers;

public sealed class LocaleHandlerTests
{
    [Fact]
    public async Task SendAsync_WhenLocalePresent_SetsAcceptLanguageHeader()
    {
        const string locale = "en-US";
        var skillRequest = new SkillRequest { Request = new LaunchRequest { Locale = locale } };
        var innerHandler = Substitute.For<HttpMessageHandler>();

        HttpRequestMessage? capturedRequest = null;
        innerHandler.ReturnsResponse(HttpStatusCode.OK, predicate: req =>
        {
            capturedRequest = req;
            return true;
        });

        var localeHandler = new LocaleHandler(() => skillRequest) { InnerHandler = innerHandler };
        using var httpClient = new HttpClient(localeHandler) { BaseAddress = new Uri("https://api.amazonalexa.com/") };

        await httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, new Uri("/test", UriKind.Relative)),
            TestContext.Current.CancellationToken);

        capturedRequest!.Headers.AcceptLanguage
            .Should().ContainSingle(h => h.Value == locale);
    }

    [Theory]
    [InlineAlexaVoxCraftAutoData(null)]
    [InlineAlexaVoxCraftAutoData("")]
    [InlineAlexaVoxCraftAutoData("   ")]
    public async Task SendAsync_WhenLocaleNullOrWhitespace_ThrowsInvalidOperationException(string? locale)
    {
        var skillRequest = new SkillRequest { Request = new LaunchRequest { Locale = locale } };
        var innerHandler = Substitute.For<HttpMessageHandler>();
        innerHandler.ReturnsResponse(HttpStatusCode.OK);

        var localeHandler = new LocaleHandler(() => skillRequest) { InnerHandler = innerHandler };
        using var httpClient = new HttpClient(localeHandler) { BaseAddress = new Uri("https://api.amazonalexa.com/") };

        var act = async () => await httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, new Uri("/test", UriKind.Relative)),
            TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Locale*");
    }

    [Fact]
    public void Constructor_WhenFactoryIsNull_ThrowsArgumentNullException()
    {
        var act = () => new LocaleHandler(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("skillRequestFactory");
    }
}