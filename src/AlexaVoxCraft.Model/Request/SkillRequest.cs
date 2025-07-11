using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request;

/// <summary>
/// Represents the complete request envelope sent from the Alexa service to a skill.
/// This is the top-level container for all Alexa skill requests and includes
/// the request data, session information, and device context.
/// </summary>
public class SkillRequest
{
    /// <summary>
    /// Gets or sets the version of the Alexa request/response specification.
    /// This indicates the format version used by the Alexa service.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the session information for this request.
    /// Contains session attributes, application data, and user information.
    /// May be null for certain request types that don't use sessions.
    /// </summary>
    [JsonPropertyName("session")]
    public Session Session { get; set; }

    /// <summary>
    /// Gets or sets the context information including device capabilities and state.
    /// Provides information about the device making the request and available interfaces.
    /// </summary>
    [JsonPropertyName("context")]
    public Context Context { get; set; }

    /// <summary>
    /// Gets or sets the actual request data containing the specific request details.
    /// This could be an intent request, launch request, session ended request, or other request types.
    /// </summary>
    [JsonPropertyName("request")]
    public Type.Request Request { get; set; }

    /// <summary>
    /// Gets the runtime type of the request for use in request routing and handling.
    /// </summary>
    /// <returns>The runtime type of the request, or null if no request is set.</returns>
    public System.Type GetRequestType()
    {
        return Request?.GetType();
    }
}