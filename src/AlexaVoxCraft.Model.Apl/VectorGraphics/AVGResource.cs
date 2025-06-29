using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.VectorGraphics;

public class AVGResource : Resource
{

    [JsonPropertyName("patterns")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string,AVG>? Patterns { get; set; }

    public void AddPattern(string key, AVG graphic)
    {
        Patterns ??= new Dictionary<string, AVG>();
        Patterns.Add(key, graphic);
    }
}