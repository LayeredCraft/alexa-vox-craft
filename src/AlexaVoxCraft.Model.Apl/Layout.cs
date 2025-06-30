﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class Layout
{
    public Layout() { }

    public Layout(params APLComponent[] items) : this((IEnumerable<APLComponent>)items) { }

    public Layout(IEnumerable<APLComponent> items)
    {
        Items = items.ToList();
    }

    [JsonPropertyName("bind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Binding>? Bindings { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; set; }

    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Parameter>? Parameters { get; set; }

    [JsonPropertyName("items")] 
    public IList<APLComponent> Items { get; set; }

    public Layout AsMain(string dataSourceKey = "payload")
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
        AlexaJsonOptions.RegisterTypeModifier<Layout>(info =>
        {
            var bindProp = info.Properties.FirstOrDefault(p => p.Name == "bind");
            if (bindProp is not null)
            {
                bindProp.ShouldSerialize = ((obj, _) =>
                {
                    var layout = (Layout)obj;
                    return layout.Bindings?.Any() ?? false;
                });
            }
            var parametersProp = info.Properties.FirstOrDefault(p => p.Name == "parameters");
            if (parametersProp is not null)
            {
                parametersProp.CustomConverter = new ParameterListConverter(true);
            }
            var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
            if (itemsProp is not null)
            {
                itemsProp.CustomConverter = new APLComponentListConverter(false);
            }
        });
    }
}