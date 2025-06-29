using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class RemoveNamespace : DataStoreCommand
{
    public const string CommandType = "REMOVE_NAMESPACE";

    public RemoveNamespace():base(CommandType){}

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; }
}