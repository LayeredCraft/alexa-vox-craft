using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Apl.Traits;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class GridSequence : ActionableComponent, IJsonSerializable<GridSequence>, IMultiChildComponent
{
    [JsonIgnore]
    internal MultiChildComponentTrait MultiChild { get; } = new();

    [JsonPropertyName("type")] public override string Type => nameof(GridSequence);

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

    [JsonPropertyName("allowForward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? AllowForward { get; set; }

    [JsonPropertyName("allowBackwards")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? AllowBackwards { get; set; }

    // IMultiChildComponent
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object>? Data
    {
        get => MultiChild.Data;
        set => MultiChild.Data = value;
    }

    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? FirstItem
    {
        get => MultiChild.FirstItem;
        set => MultiChild.FirstItem = value;
    }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? Items
    {
        get => MultiChild.Items;
        set => MultiChild.Items = value;
    }

    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent>? LastItem
    {
        get => MultiChild.LastItem;
        set => MultiChild.LastItem = value;
    }

    [JsonPropertyName("onChildrenChanged")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? OnChildrenChanged
    {
        get => MultiChild.OnChildrenChanged;
        set => MultiChild.OnChildrenChanged = value;
    }

    public new static void RegisterTypeInfo<T>() where T : GridSequence
    {
        ActionableComponent.RegisterTypeInfo<T>();
        MultiChildComponentTrait.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var childHeightsProp = info.Properties.FirstOrDefault(p => p.Name == "childHeights");
            childHeightsProp?.CustomConverter = new APLValueCollectionConverter<APLDimensionValue>(false);

            var childWidthsProp = info.Properties.FirstOrDefault(p => p.Name == "childWidths");
            childWidthsProp?.CustomConverter = new APLValueCollectionConverter<APLDimensionValue>(false);

            var onScrollProp = info.Properties.FirstOrDefault(p => p.Name == "onScroll");
            onScrollProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}