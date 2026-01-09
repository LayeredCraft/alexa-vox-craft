using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class CommandsTarget : IJsonSerializable<CommandsTarget>
{
    [JsonPropertyName("type")]
    public TargetType Type { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("items")]
    public List<string> Items { get; set; }

    public static void RegisterTypeInfo<T>() where T : CommandsTarget
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var itemsProp = info.Properties.FirstOrDefault(p => p.Name == "items");
            itemsProp?.ShouldSerialize = (obj, _) =>
            {
                var commandsTarget = (T)obj;
                return commandsTarget.Type == TargetType.Devices;
            };
            var idProp = info.Properties.FirstOrDefault(p => p.Name == "id");
            idProp?.ShouldSerialize = (obj, _) =>
            {
                var commandsTarget = (T)obj;
                return commandsTarget.Type == TargetType.User;
            };
        });
    }
}