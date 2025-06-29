using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLDocumentLink : APLDocumentReference
{
    public const string DocumentType = "Link";
    public APLDocumentLink(){}

    public APLDocumentLink(string source)
    {
        Source = source;
    }

    [JsonPropertyName("type")]
    public override string Type => DocumentType;

    [JsonPropertyName("src")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Source { get; set; }
}