using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Operation;

public class DeleteItem : Operation
{
    public DeleteItem() { }

    public DeleteItem(int index)
    {
        Index = index;
    }

    public const string OperationType = "DeleteItem";
    [JsonPropertyName("type")]
    public override string Type => OperationType;
}