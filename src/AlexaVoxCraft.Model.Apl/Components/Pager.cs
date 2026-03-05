using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Apl.Traits;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Pager : ActionableComponent, IJsonSerializable<Pager>, IMultiChildComponent
{
    [JsonIgnore]
    internal MultiChildComponentTrait MultiChild { get; } = new();

    public Pager()
    {
    }

    public Pager(params APLComponent[] items) : this((IEnumerable<APLComponent>)items)
    {
    }

    public Pager(IEnumerable<APLComponent> items)
    {
        Items = new APLValueCollection<APLComponent>(items);
    }

    [JsonPropertyName("type")]
    public override string Type => nameof(Pager);

    [JsonPropertyName("initialPage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<int?> InitialPage { get; set; }

    [JsonPropertyName("navigation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string> Navigation { get; set; }

    [JsonPropertyName("onPageChanged")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnPageChanged { get; set; }

    [JsonPropertyName("handlePageMove")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLPageMoveHandler> HandlePageMove { get; set; }

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

    public new static void RegisterTypeInfo<T>() where T : Pager
    {
        ActionableComponent.RegisterTypeInfo<T>();
        MultiChildComponentTrait.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var onPageChangedProp = info.Properties.FirstOrDefault(p => p.Name == "onPageChanged");
            onPageChangedProp?.CustomConverter = new APLCommandListConverter(false);

            var handlePageMoveProp = info.Properties.FirstOrDefault(p => p.Name == "handlePageMove");
            handlePageMoveProp?.CustomConverter = new APLValueCollectionConverter<APLPageMoveHandler>(false);
        });
    }
}