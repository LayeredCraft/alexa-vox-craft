using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public abstract class ListDataDirective : IDirective, IJsonSerializable<ListDataDirective>
{
    [JsonPropertyName("type")] public abstract string Type { get; }

    [JsonPropertyName("token")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Token { get; set; }

    [JsonPropertyName("correlationToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CorrelationToken { get; set; }

    [JsonPropertyName("listId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ListId { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<object>? Items { get; set; } = new List<object>();

    public static void RegisterTypeInfo<T>() where T : ListDataDirective
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
            itemsProp?.ShouldSerialize = (obj, _) =>
            {
                var listDataDirective = (T)obj;
                return listDataDirective.Items?.Any() ?? false;
            };
        });
    }
}