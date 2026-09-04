using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive.Templates;

public class TemplateImage
{
    [JsonPropertyName("contentDescription"), JsonRequired]
    public string ContentDescription { get; set; } = null!;

    [JsonPropertyName("sources")] public List<ImageSource> Sources { get; set; } = [];
}