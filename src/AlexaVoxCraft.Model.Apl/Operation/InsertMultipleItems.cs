using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Operation;

public class InsertMultipleItems : Operation
{
    public InsertMultipleItems() { }

    public InsertMultipleItems(int index, params object[] items)
    {
        Index = index;
        Items = items;
    }

    public const string OperationType = "InsertMultipleItems";
    [JsonPropertyName("type")]
    public override string Type => OperationType;
    [JsonPropertyName("items")]
    public object[] Items { get; set; }
}