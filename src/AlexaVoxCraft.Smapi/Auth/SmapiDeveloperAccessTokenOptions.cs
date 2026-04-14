using System.ComponentModel.DataAnnotations;

namespace AlexaVoxCraft.Smapi.Auth;

/// <summary>
/// Configuration options for SMAPI developer authentication.
/// </summary>
public sealed record SmapiDeveloperAccessTokenOptions
{
    /// <summary>
    /// Gets the Login With Amazon (LWA) client identifier used for SMAPI developer access.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the Login With Amazon (LWA) client secret used for SMAPI developer access.
    /// </summary>
    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets the long-lived LWA refresh token used to obtain short-lived access tokens.
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
