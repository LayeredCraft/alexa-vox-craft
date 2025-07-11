using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response;

/// <summary>
/// Represents the complete response envelope sent from a skill back to the Alexa service.
/// This is the top-level container for all skill responses and includes the response data,
/// session attributes, and version information.
/// </summary>
public class SkillResponse
{
    /// <summary>
    /// Gets or sets the version of the Alexa request/response specification.
    /// This must match the version format expected by the Alexa service.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the session attributes to persist for the next request in the same session.
    /// These attributes are stored by Alexa and returned in subsequent requests within the same session.
    /// Set to null if no session attributes need to be persisted.
    /// </summary>
    [JsonPropertyName("sessionAttributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? SessionAttributes { get; set; }

    /// <summary>
    /// Gets or sets the response body containing the actual response data.
    /// This includes speech output, cards, directives, and session management information.
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("response")]
    public ResponseBody Response { get; set; }
}