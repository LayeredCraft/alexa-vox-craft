using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Operation;

public class InsertItem : Operation
{
    public InsertItem() { }

    public InsertItem(int index, object item)
    {
        Index = index;
        Item = item;
    }

    public const string OperationType = "InsertItem";

    [JsonPropertyName("type")]
    public override string Type => OperationType;

    [JsonPropertyName("item")]
    public object Item { get; set; }
}