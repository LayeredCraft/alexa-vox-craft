using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.Response.Directive;

public class DialogConfirmIntent : IDirective
{
    public const string DirectiveType = "Dialog.ConfirmIntent";

    [JsonPropertyName("type")]
    public string Type => DirectiveType;

    [JsonPropertyName("updatedIntent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Intent UpdatedIntent { get; set; }
}