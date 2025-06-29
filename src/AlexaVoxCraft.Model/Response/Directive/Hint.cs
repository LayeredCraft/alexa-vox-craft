using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Response.Directive;

public class Hint
{
    public Hint()
    {
    }

    public Hint(string hintText, string textType = TextType.Plain)
    {
        Text = hintText;
        Type = textType;
    }

    [JsonPropertyName("type")]
    public string Type { get; set; }
        
    [JsonPropertyName("text")]
    public string Text { get; set; }
}