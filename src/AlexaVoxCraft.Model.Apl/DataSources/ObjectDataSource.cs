using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataSources;

public class ObjectDataSource:APLDataSource
{
    public const string DataSourceType = "object";
    [JsonPropertyName("type")] public override string Type => DataSourceType;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    [JsonPropertyName("objectID")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ObjectId { get; set; }

    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; set; }

    [JsonPropertyName("properties")]
    public virtual Dictionary<string,object> Properties { get; set; }

    [JsonExtensionData]
    public virtual Dictionary<string, object> TopLevelData { get; set; }

    [JsonPropertyName("transformers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<APLTransformer>? Transformers { get; set; }
}