﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class GridSequence : ActionableComponent, IJsonSerializable<GridSequence>
{
    [JsonPropertyName("type")] public override string Type => nameof(GridSequence);

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<object>>? Data { get; set; }

    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<List<APLComponent>>? FirstItem { get; set; }

    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<List<APLComponent>>? LastItem { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLComponent>>? Items { get; set; }

    [JsonPropertyName("childHeights")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLDimensionValue>>? ChildHeights { get; set; }

    [JsonPropertyName("childWidths")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLDimensionValue>>? ChildWidths { get; set; }

    [JsonPropertyName("numbered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Numbered { get; set; }

    [JsonPropertyName("onScroll")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnScroll { get; set; }

    [JsonPropertyName("scrollDirection")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<ScrollDirection?>? ScrollDirection { get; set; }

    [JsonPropertyName("snap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<Snap?>? Snap { get; set; }

    public new static void RegisterTypeInfo<T>() where T : GridSequence
    {
        ActionableComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var dataProp = info.Properties.FirstOrDefault(p => p.Name == "data");
            if (dataProp is not null)
            {
                dataProp.CustomConverter = new GenericSingleOrListConverter<object>(false);
            }

            var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
            if (itemsProp is not null)
            {
                itemsProp.CustomConverter = new APLComponentListConverter(false);
            }

            var childHeightsProp = info.Properties.FirstOrDefault(p => p.Name == "childHeights");
            if (childHeightsProp is not null)
            {
                childHeightsProp.CustomConverter = new GenericSingleOrListConverter<APLDimensionValue>(false);
            }

            var childWidthsProp = info.Properties.FirstOrDefault(p => p.Name == "childWidths");
            if (childWidthsProp is not null)
            {
                childWidthsProp.CustomConverter = new GenericSingleOrListConverter<APLDimensionValue>(false);
            }

            var onScrollProp = info.Properties.FirstOrDefault(p => p.Name == "onScroll");
            if (onScrollProp is not null)
            {
                onScrollProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}