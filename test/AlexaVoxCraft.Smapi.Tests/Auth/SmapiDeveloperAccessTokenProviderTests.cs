using System.Net;
using AlexaVoxCraft.Smapi.Auth;
using AlexaVoxCraft.Smapi.Tests.TestKit;
using Compono;
using Compono.Http;
using Compono.XunitV3;

namespace AlexaVoxCraft.Smapi.Tests.Auth;

public sealed class SmapiDeveloperAccessTokenProviderTests
{
    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAccessTokenAsync_FirstCall_ReturnsToken(
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };
        handler.OnPost(Match.Any<string>()).RespondJson(lwaResponse);

        var token = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token.Should().Be(accessToken);
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAccessTokenAsync_CalledTwiceWithinExpiry_ReturnsSameTokenWithoutSecondRequest(
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };
        var registration = handler.OnPost(Match.Any<string>()).RespondJson(lwaResponse);

        var token1 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);
        var token2 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token1.Should().Be(accessToken);
        token2.Should().Be(accessToken);
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAccessTokenAsync_CallsCorrectEndpoint(
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };
        var expectedUri = "https://api.amazon.com/auth/o2/token";

        var registration = handler.When(req => req.RequestUri?.ToString() == expectedUri && req.Method == HttpMethod.Post)
            .RespondJson(lwaResponse);

        var token = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token.Should().NotBeNullOrEmpty();
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAccessTokenAsync_SendsFormUrlEncodedContent(
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };

        var registration = handler.When(req => req.Content is FormUrlEncodedContent)
            .RespondJson(lwaResponse);

        var token = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token.Should().Be(accessToken);
        registration.Verify().Once();
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public async Task GetAccessTokenAsync_WhenTokenExpiresSoon_RefreshesToken(
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider,
        string firstToken)
    {
        var firstResponse = new { access_token = firstToken, expires_in = 1 };
        var registration = handler.OnPost(Match.Any<string>()).RespondJson(firstResponse);

        var token1 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        var token2 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token1.Should().Be(firstToken);
        token2.Should().Be(firstToken);
        registration.Verify().Exactly(2);
    }

    // AllowMultiple = false on ComposeAttribute (unlike [InlineData]) means the three null/empty/
    // whitespace cases each need their own [Compose<SmapiHttpTestProfile>(value)] - one inline
    // value per attribute instance, one attribute instance per method.
    [Theory, Compose<SmapiHttpTestProfile>((string?)null)]
    public async Task GetAccessTokenAsync_WhenResponseMissingAccessToken_Null_ThrowsInvalidOperationException(
        string? token,
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider) =>
        await AssertThrowsForMissingAccessToken(token, handler, provider);

    [Theory, Compose<SmapiHttpTestProfile>("")]
    public async Task GetAccessTokenAsync_WhenResponseMissingAccessToken_Empty_ThrowsInvalidOperationException(
        string? token,
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider) =>
        await AssertThrowsForMissingAccessToken(token, handler, provider);

    [Theory, Compose<SmapiHttpTestProfile>("   ")]
    public async Task GetAccessTokenAsync_WhenResponseMissingAccessToken_Whitespace_ThrowsInvalidOperationException(
        string? token,
        [Shared] HttpTestHarness handler,
        SmapiDeveloperAccessTokenProvider provider) =>
        await AssertThrowsForMissingAccessToken(token, handler, provider);

    private static async Task AssertThrowsForMissingAccessToken(
        string? token, HttpTestHarness handler, SmapiDeveloperAccessTokenProvider provider)
    {
        var invalidResponse = new { access_token = token, expires_in = 3600 };
        handler.OnPost(Match.Any<string>()).RespondJson(invalidResponse);

        var act = async () => await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*access_token missing*");
    }

    [Theory, Compose<SmapiHttpTestProfile>]
    public void Dispose_CanBeCalledMultipleTimes(
        SmapiDeveloperAccessTokenProvider provider)
    {
        var act = () =>
        {
            provider.Dispose();
            provider.Dispose();
        };

        act.Should().NotThrow();
    }
}
