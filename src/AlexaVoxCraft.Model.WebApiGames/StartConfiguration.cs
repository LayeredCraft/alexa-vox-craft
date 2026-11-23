using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// Configuration for the <c>Alexa.Presentation.HTML.Start</c> directive. Amazon requires the
/// configuration object, so it is always emitted with the default timeout when none is supplied.
/// </summary>
public sealed class StartConfiguration
{
    /// <summary>Initializes a new configuration with the given timeout in seconds.</summary>
    /// <param name="timeoutInSeconds">
    /// The number of seconds the web app can run without user interaction. Defaults to 30; values above
    /// 300 are rejected. No minimum is enforced.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="timeoutInSeconds"/> is greater than 300.</exception>
    public StartConfiguration(int timeoutInSeconds = 30)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(timeoutInSeconds, 300);
        TimeoutInSeconds = timeoutInSeconds;
    }

    /// <summary>Gets the number of seconds the web app can run without user interaction.</summary>
    [JsonPropertyName("timeoutInSeconds")]
    public int TimeoutInSeconds { get; }
}
