using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Response.Directive;

public class ListItem
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    [JsonPropertyName("image")]
    public TemplateImage Image { get; set; } = null!;

    [JsonPropertyName("textContent")]
    public TemplateContent Content { get; set; } = null!;
}