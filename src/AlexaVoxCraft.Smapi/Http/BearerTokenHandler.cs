using System.Net.Http.Headers;
using AlexaVoxCraft.Smapi.Auth;

namespace AlexaVoxCraft.Smapi.Http;

public sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BearerTokenHandler"/> class.
    /// </summary>
    /// <param name="accessTokenProvider">The access token provider.</param>
    public BearerTokenHandler(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider
                               ?? throw new ArgumentNullException(nameof(accessTokenProvider));
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _accessTokenProvider
            .GetAccessTokenAsync(cancellationToken)
            .ConfigureAwait(false);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}