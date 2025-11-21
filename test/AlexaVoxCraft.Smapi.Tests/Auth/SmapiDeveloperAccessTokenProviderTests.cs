using System.Net;
using AlexaVoxCraft.Smapi.Auth;
using AlexaVoxCraft.Smapi.Tests.TestKit.Attributes;
using AlexaVoxCraft.Smapi.Tests.TestKit.Extensions;

namespace AlexaVoxCraft.Smapi.Tests.Auth;

public sealed class SmapiDeveloperAccessTokenProviderTests
{
    [Theory, ClientAutoData]
    public async Task GetAccessTokenAsync_FirstCall_ReturnsToken(
        [Frozen] HttpMessageHandler handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };
        handler.ReturnsResponse(HttpStatusCode.OK, lwaResponse);

        var token = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token.Should().Be(accessToken);
    }

    [Theory, ClientAutoData]
    public async Task GetAccessTokenAsync_CalledTwiceWithinExpiry_ReturnsSameTokenWithoutSecondRequest(
        [Frozen] HttpMessageHandler handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };
        handler.ReturnsResponse(HttpStatusCode.OK, lwaResponse);

        var token1 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);
        var token2 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token1.Should().Be(accessToken);
        token2.Should().Be(accessToken);
        handler.ReceivedCalls().Should().HaveCount(1);
    }

    [Theory, ClientAutoData]
    public async Task GetAccessTokenAsync_CallsCorrectEndpoint(
        [Frozen] HttpMessageHandler handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };
        var expectedUri = "https://api.amazon.com/auth/o2/token";

        handler.ReturnsResponse(HttpStatusCode.OK, lwaResponse,
            req => req.RequestUri?.ToString() == expectedUri && req.Method == HttpMethod.Post);

        var token = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token.Should().NotBeNullOrEmpty();
    }

    [Theory, ClientAutoData]
    public async Task GetAccessTokenAsync_SendsFormUrlEncodedContent(
        [Frozen] HttpMessageHandler handler,
        SmapiDeveloperAccessTokenProvider provider,
        string accessToken)
    {
        var lwaResponse = new { access_token = accessToken, expires_in = 3600 };

        handler.ReturnsResponse(HttpStatusCode.OK, lwaResponse,
            req => req.Content is FormUrlEncodedContent);

        var token = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token.Should().Be(accessToken);
        handler.Received();
    }

    [Theory, ClientAutoData]
    public async Task GetAccessTokenAsync_WhenTokenExpiresSoon_RefreshesToken(
        [Frozen] HttpMessageHandler handler,
        SmapiDeveloperAccessTokenProvider provider,
        string firstToken)
    {
        var firstResponse = new { access_token = firstToken, expires_in = 1 };
        handler.ReturnsResponse(HttpStatusCode.OK, firstResponse);

        var token1 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        var token2 = await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        token1.Should().Be(firstToken);
        token2.Should().Be(firstToken);
        handler.ReceivedCalls().Should().HaveCount(2);
    }

    [Theory] 
    [InlineClientAutoData((string?)null)]
    [InlineClientAutoData("")]
    [InlineClientAutoData("   ")]
    public async Task GetAccessTokenAsync_WhenResponseMissingAccessToken_ThrowsInvalidOperationException(
        string? token,
        [Frozen] HttpMessageHandler handler,
        SmapiDeveloperAccessTokenProvider provider)
    {
        var invalidResponse = new { access_token = token, expires_in = 3600 };
        handler.ReturnsResponse(HttpStatusCode.OK, invalidResponse);

        var act = async () => await provider.GetAccessTokenAsync(TestContext.Current.CancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*access_token missing*");
    }

    [Theory, ClientAutoData]
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