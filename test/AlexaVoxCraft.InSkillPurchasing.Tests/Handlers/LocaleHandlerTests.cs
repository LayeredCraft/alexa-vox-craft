using System.Net;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using Compono.Http;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Handlers;

public sealed class LocaleHandlerTests
{
    [Fact]
    public async Task SendAsync_WhenLocalePresent_SetsAcceptLanguageHeader()
    {
        const string locale = "en-US";
        var skillRequest = new SkillRequest { Request = new LaunchRequest { Locale = locale } };
        using var innerHandler = new TestHttpHandler();
        innerHandler.When(_ => true).Respond(HttpStatusCode.OK);

        var localeHandler = new LocaleHandler(() => skillRequest) { InnerHandler = innerHandler };
        using var httpClient = new HttpClient(localeHandler) { BaseAddress = new Uri("https://api.amazonalexa.com/") };

        await httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, new Uri("/test", UriKind.Relative)),
            TestContext.Current.CancellationToken);

        // Replaces the old predicate-side-effect capture (a matcher closure smuggling the request
        // out via a captured local variable) with a real, first-class read from the handler's own
        // request log - ADR-0051 "Kept separate: global request-log inspection".
        innerHandler.Requests.Should().ContainSingle()
            .Which.Headers.AcceptLanguage.Should().ContainSingle(h => h.Value == locale);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SendAsync_WhenLocaleNullOrWhitespace_ThrowsInvalidOperationException(string? locale)
    {
        var skillRequest = new SkillRequest { Request = new LaunchRequest { Locale = locale! } };
        using var innerHandler = new TestHttpHandler();
        innerHandler.When(_ => true).Respond(HttpStatusCode.OK);

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
