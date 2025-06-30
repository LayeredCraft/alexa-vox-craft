using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Operation;

public class DeleteMultipleItems : Operation
{
    public DeleteMultipleItems() { }

    public DeleteMultipleItems(int index, int count)
    {
        Index = index;
    }

    public const string OperationType = "DeleteMultipleItems";
    [JsonPropertyName("type")]
    public override string Type => OperationType;

    [JsonPropertyName("count")]
    public int Count { get; set; }
}