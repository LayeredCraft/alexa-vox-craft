using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class Pager : ActionableComponent, IJsonSerializable<Pager>
{
    public Pager()
    {
    }

    public Pager(params APLComponent[] items) : this((IEnumerable<APLComponent>)items)
    {
    }

    public Pager(IEnumerable<APLComponent> items)
    {
        Items = [..items];
    }

    [JsonPropertyName("type")]
    public override string Type => nameof(Pager);

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<object> Data { get; set; }

    [JsonPropertyName("firstItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent> FirstItem { get; set; }

    [JsonPropertyName("lastItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent> LastItem { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLComponent> Items { get; set; }

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

    [JsonPropertyName("onChildrenChanged")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand> OnChildrenChanged { get; set; }

    public new static void RegisterTypeInfo<T>() where T : Pager
    {
        ActionableComponent.RegisterTypeInfo<T>();
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
            itemsProp?.CustomConverter = new APLValueCollectionConverter<APLComponent>(false);

            var onPageChangedProp = info.Properties.FirstOrDefault(p => p.Name == "onPageChanged");
            onPageChangedProp?.CustomConverter = new APLCommandListConverter(false);

            var handlePageMoveProp = info.Properties.FirstOrDefault(p => p.Name == "handlePageMove");
            handlePageMoveProp?.CustomConverter = new APLValueCollectionConverter<APLPageMoveHandler>(false);

            var onChildrenChangedProp = info.Properties.FirstOrDefault(p => p.Name == "onChildrenChanged");
            onChildrenChangedProp?.CustomConverter = new APLCommandListConverter(false);
        });
    }
}