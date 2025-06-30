using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Audio;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public interface IJsonSerializable<TType>
{
    static abstract void RegisterTypeInfo<T>() where T : TType;
}
public abstract class APLComponentBase : IJsonSerializable<APLComponentBase>
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public abstract string? Type { get; }

    [JsonPropertyName("bind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Binding>? Bindings { get; set; }
    
    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? When { get; set; }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AudioDuration? Duration { get; set; }
    
    public static void RegisterTypeInfo<T>() where T : APLComponentBase
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var bindProp = info.Properties.FirstOrDefault(p => p.Name == "bind");
            if (bindProp is not null)
            {
                bindProp.ShouldSerialize = ((obj, _) =>
                {
                    var layout = (T)obj;
                    return layout.Bindings?.Any() ?? false;
                });
            }
        });
    }
}