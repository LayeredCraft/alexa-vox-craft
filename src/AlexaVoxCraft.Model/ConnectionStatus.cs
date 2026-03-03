using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model;

/// <summary>
/// Represents the status returned in a <c>Connections.Response</c> request, containing an HTTP-style status code and descriptive message.
/// </summary>
public class ConnectionStatus
{
    /// <summary>Initializes a new default instance of <see cref="ConnectionStatus"/>.</summary>
    public ConnectionStatus() { }

    /// <summary>
    /// Initializes a new instance of <see cref="ConnectionStatus"/> with the specified code and message.
    /// </summary>
    /// <param name="code">The HTTP-style status code (e.g., 200 for success).</param>
    /// <param name="message">A human-readable description of the status.</param>
    public ConnectionStatus(int code, string message)
    {
        Code = code;
        Message = message;
    }

    /// <summary>Gets or sets the HTTP-style status code. Supports reading from a JSON string or number.</summary>
    [JsonPropertyName("code")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Code { get; set; }

    /// <summary>Gets or sets the human-readable description of the connection status.</summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}