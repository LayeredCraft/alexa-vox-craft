using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class RuntimeErrorRequest : Request.Type.Request
{
    public const string RequestType = "Alexa.Presentation.APL.RuntimeError";

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }

    [JsonPropertyName("errors")] public APLError[] Errors { get; set; }
}