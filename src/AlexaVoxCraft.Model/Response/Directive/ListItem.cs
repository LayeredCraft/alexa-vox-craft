using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Response.Directive;

public class ListItem
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
        
    [JsonPropertyName("image")]
    public TemplateImage Image { get; set; }
        
    [JsonPropertyName("textContent")]
    public TemplateContent Content { get; set; }
}