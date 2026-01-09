using System.Collections.Generic;
using System.Text.Json.Serialization;

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
    }
}