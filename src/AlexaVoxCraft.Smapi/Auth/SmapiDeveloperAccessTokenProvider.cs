using System.Text.Json;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.Smapi.Auth;

public sealed class SmapiDeveloperAccessTokenProvider : IAccessTokenProvider, IDisposable
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SmapiDeveloperAccessTokenOptions _options;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _currentToken;
    private DateTimeOffset _expiresAt;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmapiDeveloperAccessTokenProvider"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory used to create HTTP clients.</param>
    /// <param name="options">The options containing LWA developer credentials.</param>
    public SmapiDeveloperAccessTokenProvider(IHttpClientFactory httpClientFactory,
        IOptions<SmapiDeveloperAccessTokenOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_options.ClientId))
        {
            throw new ArgumentException("ClientId must be configured.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            throw new ArgumentException("ClientSecret must be configured.", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(_options.RefreshToken))
        {
            throw new ArgumentException("RefreshToken must be configured.", nameof(options));
        }
        
    }
    
    /// <inheritdoc />
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Fast path: token still valid for at least 60 more seconds.
        if (_currentToken is not null &&
            _expiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
        {
            return _currentToken;
        }

        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            // Re-check after taking the lock to avoid refresh stampede.
            if (_currentToken is not null &&
                _expiresAt > DateTimeOffset.UtcNow.AddMinutes(1))
            {
                return _currentToken;
            }

            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = _options.RefreshToken,
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.amazon.com/auth/o2/token");
            request.Content = content;

            var httpClient = _httpClientFactory.CreateClient();
            
            using var response = await httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            _currentToken = root.GetProperty("access_token").GetString()
                           ?? throw new InvalidOperationException("access_token missing in LWA response.");

            var expiresIn = root.GetProperty("expires_in").GetInt32();
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            return _currentToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _lock.Dispose();
    }
}