using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.ConnectionTasks;

[JsonConverter(typeof(ConnectionTaskConverter))]
public interface IConnectionTask    
{
    [JsonIgnore]
    string ConnectionUri { get; }
}