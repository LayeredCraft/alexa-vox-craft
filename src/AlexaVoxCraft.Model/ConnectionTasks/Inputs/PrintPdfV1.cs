using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class PrintPdfV1 : IConnectionTask
{
    public const string ConnectionType = "PrintPDFRequest";
    public const string VersionNumber = "1";
    public const string ConnectionKey = $"{ConnectionType}/{VersionNumber}";

    public const string AssociatedUri = "connection://AMAZON.PrintPDF/1";

    [JsonIgnore] public string ConnectionUri => AssociatedUri;

    [JsonPropertyName("@type")] public string Type => ConnectionType;

    [JsonPropertyName("context")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConnectionTaskContext Context { get; set; }

    [JsonPropertyName("@version")] public string Version => VersionNumber;

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("url")] public string Url { get; set; }
}