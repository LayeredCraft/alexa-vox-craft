using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class PutNamespace : DataStoreCommand
{
    public const string CommandType = "PUT_NAMESPACE";

    public PutNamespace() : base(CommandType){}

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; }
}