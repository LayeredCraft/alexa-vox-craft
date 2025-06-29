using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class JsonDirective:IDirective
{
    public JsonDirective() { }

    [JsonConstructor]
    public JsonDirective(string type)
    {
        Type = type;
    }

    [JsonPropertyName("type")]
    public string Type { get; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> Properties { get; set; } = new();
}