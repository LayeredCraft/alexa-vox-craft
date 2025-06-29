using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive.Templates;

public class TemplateText
{
    [JsonPropertyName("text")]
    [JsonRequired]
    public string Text { get; set; }

    [JsonPropertyName("type")]
    [JsonRequired]
    public string Type { get; set; }
}