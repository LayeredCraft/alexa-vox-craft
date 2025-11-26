namespace AlexaVoxCraft.Smapi.Auth;

/// <summary>
/// Provides OAuth access tokens for authenticated HTTP requests.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Gets a valid access token, refreshing it if necessary.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}