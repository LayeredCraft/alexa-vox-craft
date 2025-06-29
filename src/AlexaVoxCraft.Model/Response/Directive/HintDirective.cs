using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Response.Directive;

public class HintDirective:IDirective
{
    public const string DirectiveType = "Hint";

    public HintDirective()
    {
    }

    public HintDirective(string hintText, string textType = TextType.Plain)
    {
        Hint = new Hint(hintText, textType);
    }

    [JsonPropertyName("type")]
    public string Type => DirectiveType;
        
    [JsonPropertyName("hint")]
    public Hint Hint { get; set; }
}