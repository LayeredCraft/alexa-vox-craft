using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// The resource the web app loads when a game starts. Serialized as the <c>request</c> property of an
/// <c>Alexa.Presentation.HTML.Start</c> directive.
/// </summary>
public sealed class StartRequest
{
    /// <summary>Initializes a new start request for the given resource.</summary>
    /// <param name="uri">The absolute HTTPS URI of the web app entry point.</param>
    /// <param name="headers">Optional headers sent to the web app.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uri"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="uri"/> is not an absolute HTTPS URI.</exception>
    public StartRequest(Uri uri, IReadOnlyDictionary<string, string>? headers = null)
    {
        ArgumentNullException.ThrowIfNull(uri);

        if (!uri.IsAbsoluteUri || !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The start request URI must be an absolute HTTPS URI.", nameof(uri));
        }

        Uri = uri;
        Headers = headers;
    }

    /// <summary>Gets the absolute HTTPS URI of the web app entry point.</summary>
    [JsonPropertyName("uri")]
    public Uri Uri { get; }

    /// <summary>Gets the HTTP method; always <c>GET</c>.</summary>
    [JsonPropertyName("method")]
    public string Method => "GET";

    /// <summary>Gets the optional headers sent to the web app; omitted from the wire when null.</summary>
    [JsonPropertyName("headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string>? Headers { get; }
}
