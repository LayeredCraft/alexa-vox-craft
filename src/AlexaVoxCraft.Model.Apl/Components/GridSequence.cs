using System.Collections.Generic;
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
    public APLValueCollection<object>? Data { get; set; }

    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? FirstItem { get; set; }

    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? LastItem { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? Items { get; set; }

    [JsonPropertyName("childHeights")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLDimensionValue>? ChildHeights { get; set; }

    [JsonPropertyName("childWidths")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLDimensionValue>? ChildWidths { get; set; }

    [JsonPropertyName("numbered")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?>? Numbered { get; set; }

    [JsonPropertyName("onScroll")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnScroll { get; set; }

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
            dataProp?.CustomConverter = new APLValueCollectionConverter<object>(false);

            var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
            itemsProp?.CustomConverter = new APLValueCollectionConverter<APLComponent>(false);

            var childHeightsProp = info.Properties.FirstOrDefault(p => p.Name == "childHeights");
            childHeightsProp?.CustomConverter = new APLValueCollectionConverter<APLDimensionValue>(false);

            var childWidthsProp = info.Properties.FirstOrDefault(p => p.Name == "childWidths");
            childWidthsProp?.CustomConverter = new APLValueCollectionConverter<APLDimensionValue>(false);

            var onScrollProp = info.Properties.FirstOrDefault(p => p.Name == "onScroll");
            onScrollProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}