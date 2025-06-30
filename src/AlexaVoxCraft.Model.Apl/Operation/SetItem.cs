using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Operation;

public class SetItem : Operation
{
    public SetItem() { }

    public SetItem(int index, object item)
    {
        Index = index;
        Item = item;
    }

    public const string OperationType = "SetItem";
    [JsonPropertyName("type")]
    public override string Type => OperationType;

    [JsonPropertyName("item")]
    public object Item { get; set; }
}