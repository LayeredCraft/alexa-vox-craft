using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public class PrintImageV1 : IConnectionTask
{
    public const string ConnectionType = "PrintImageRequest";
    public const string VersionNumber = "1";
    public const string ConnectionKey = $"{ConnectionType}/{VersionNumber}";

    public const string AssociatedUri = "connection://AMAZON.PrintImage/1";
    [JsonIgnore]
    public string ConnectionUri => AssociatedUri;

    [JsonPropertyName("@type")] public string Type => ConnectionType;

    [JsonPropertyName("@version")] public string Version => VersionNumber;

    [JsonPropertyName("context")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ConnectionTaskContext Context { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("imageType"),JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<PrintImageV1Type>))]
    public PrintImageV1Type ImageV1Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public enum PrintImageV1Type
{
    JPG,
    JPEG
}