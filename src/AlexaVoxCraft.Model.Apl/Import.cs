using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class Import : IEquatable<Import>, IJsonSerializable<Import>
{
    public Import()
    {
    }

    public Import(string name, string version)
    {
        Name = name;
        Version = version;
    }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Version { get; set; }

    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Source { get; set; }

    [JsonPropertyName("when")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<bool?>? When { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImportType? Type { get; set; }

    [JsonPropertyName("loadAfter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<string>? LoadAfter { get; set; } = new List<string>();

    public static Import AlexaStyles => new Import("alexa-styles", "1.6.0");
    public static Import AlexaViewportProfiles => new Import("alexa-viewport-profiles", "1.6.0");
    public static Import AlexaLayouts => new Import("alexa-layouts", "1.7.0");
    public static Import AlexaIcon => new Import("alexa-icon", "1.0.0");

    public void Into(APLDocument document)
    {

        if (document.Imports == null)
        {
            document.Imports = new List<Import>();
        }
        else if (document.Imports.Contains(this))
        {
            return;
        }

        document.Imports.Add(this);
    }

    public bool Equals(Import other)
    {
        if (other == null)
        {
            return false;
        }

        return other.Name == Name && other.Version == Version;
    }


    public static void RegisterTypeInfo<T>() where T : Import
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var loadAfterProp = info.Properties.FirstOrDefault(p => p.Name == "loadAfter");
            if (loadAfterProp is null) return;
            loadAfterProp.ShouldSerialize = (obj, _) =>
            {
                var import = (T)obj;
                return import.LoadAfter?.Any() ?? false;
            };
            loadAfterProp.CustomConverter = new StringOrArrayConverter(false);
        });
    }
}