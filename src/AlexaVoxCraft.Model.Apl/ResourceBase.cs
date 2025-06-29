using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public abstract class ResourceBase
{
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? When { get; set; }

    [JsonPropertyName("strings")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Strings { get; set; }

    [JsonPropertyName("numbers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, APLValue<double?>>? Numbers { get; set; }

    [JsonPropertyName("booleans")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Booleans { get; set; }

    [JsonPropertyName("arrays")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Arrays { get; set; }

    [JsonPropertyName("maps")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Maps { get; set; }

    public void AddString(string key, string expression)
    {
        Strings ??= new Dictionary<string, string>();
        Strings.Add(key, expression);
    }

    public void AddNumber(string key, APLValue<double?> resource)
    {
        Numbers ??= new Dictionary<string, APLValue<double?>>();
        Numbers.Add(key, resource);
    }

    public void AddBoolean(string key, string expression)
    {
        Booleans ??= new Dictionary<string, string>();
        Booleans.Add(key, expression);
    }

    public void AddArray(string key, string expression)
    {
        Arrays ??= new Dictionary<string, string>();
        Arrays.Add(key, expression);
    }

    public void AddMaps(string key, string expression)
    {
        Maps ??= new Dictionary<string, string>();
        Maps.Add(key, expression);
    }
}