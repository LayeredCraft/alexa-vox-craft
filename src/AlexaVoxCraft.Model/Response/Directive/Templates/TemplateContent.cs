using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive.Templates;

public class TemplateContent
{
    [JsonPropertyName("primaryText")]
    [JsonRequired]
    public TemplateText Primary { get; set; }

    [JsonPropertyName("secondaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TemplateText? Secondary { get; set; }

    [JsonPropertyName("tertiaryText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TemplateText? Tertiary { get; set; }
}