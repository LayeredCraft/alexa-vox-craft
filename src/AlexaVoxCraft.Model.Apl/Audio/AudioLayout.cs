using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Audio;

public class AudioLayout
{
    public AudioLayout() { }

    public AudioLayout(params APLAComponent[] items) : this((IEnumerable<APLAComponent>)items) { }

    public AudioLayout(IEnumerable<APLAComponent> items)
    {
        Items = items.ToList();
    }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Parameter>? Parameters { get; set; }

    [JsonPropertyName("items")]
    public IList<APLAComponent> Items { get; set; }

    public AudioLayout AsMain(string dataSourceKey = "payload")
    {
        if (Parameters == null)
        {
            Parameters = new List<Parameter>();
        }

        if (Parameters.All(p => string.Equals(p.Name, dataSourceKey, StringComparison.OrdinalIgnoreCase)))
        {
            Parameters.Add(new Parameter(dataSourceKey));
        }

        return this;
    }
    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<AudioLayout>(typeInfo =>
        {
            var parameterProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "parameters");
            parameterProp?.CustomConverter = new ParameterListConverter(true);
            var itemsProp = typeInfo.Properties.FirstOrDefault(p => p.Name == "items");
            itemsProp?.CustomConverter = new APLAComponentListConverter(true);
        });
    }
}