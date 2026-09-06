using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response;

public class ProgressiveResponseRequest
{
    public ProgressiveResponseRequest()
    {
    }

    public ProgressiveResponseRequest(ProgressiveResponseHeader header, IProgressiveResponseDirective directive)
    {
        Header = header;
        Directive = directive;
    }

    [JsonPropertyName("header")]
    public ProgressiveResponseHeader Header { get; set; } = null!;

    [JsonPropertyName("directive")]
    public IProgressiveResponseDirective Directive { get; set; } = null!;
}