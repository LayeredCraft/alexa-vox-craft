using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Response.Directive;

public class DialogDelegate:IDirective
{
    public const string DirectiveType = "Dialog.Delegate";

    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    [JsonPropertyName("updatedIntent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Intent UpdatedIntent { get; set; }
}