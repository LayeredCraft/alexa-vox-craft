using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Commands;

public class Reinflate : APLCommand, IJsonSerializable<Reinflate>
{
    [JsonPropertyName("type")]
    public override string Type => nameof(Reinflate);

    [JsonPropertyName("preservedSequencers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<string>? PreservedSequencers { get; set; }

    public static void RegisterTypeInfo<T>() where T : Reinflate
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var prop = info.Properties.FirstOrDefault(p => p.Name == "preservedSequencers");
            prop?.CustomConverter = new APLValueCollectionConverter<string>(alwaysOutputArray: true);
        });
    }
}