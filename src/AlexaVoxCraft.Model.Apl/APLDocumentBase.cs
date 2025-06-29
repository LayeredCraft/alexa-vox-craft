using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public abstract class APLDocumentBase : APLDocumentReference, IJsonSerializable<APLDocumentBase>
{
    protected APLDocumentBase()
    {
        Version = APLDocumentVersion.V1;
    }

    protected APLDocumentBase(APLDocumentVersion version)
    {
        Version = version;
    }

    [JsonIgnore]
    public APLDocumentVersion Version
    {
        get => EnumParser.ToEnum(VersionString, APLDocumentVersion.Unknown);
        set => VersionString = EnumParser.ToEnumString(typeof(APLDocumentVersion), value);
    }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? Description { get; set; }

    [JsonPropertyName("layouts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Layout>? Layouts { get; set; }

    [JsonPropertyName("resources")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Resource>? Resources { get; set; }

    [JsonPropertyName("mainTemplate")] public Layout MainTemplate { get; set; }

    [JsonPropertyName("onMount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnMount { get; set; }

    [JsonPropertyName("onConfigChange")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>>? OnConfigChange { get; set; }

    [JsonPropertyName("settings")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDocumentSettings? Settings { get; set; }

    [JsonPropertyName("extensions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLExtension>>? Extensions { get; set; } = new List<APLExtension>();

    [JsonPropertyName("environment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLDocumentEnvironment? Environment { get; set; }

    [JsonExtensionData] public Dictionary<string, object> Handlers { get; set; }

    public void AddHandler(string name, APLValue<IList<APLCommand>> commands)
    {
        if (Handlers == null)
        {
            Handlers = new Dictionary<string, object>();
        }

        Handlers.Add(name, commands);
    }

    public static void RegisterTypeInfo<T>() where T : APLDocumentBase
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var extensionsProp = info.Properties.FirstOrDefault(p => p.Name == "extensions");
            if (extensionsProp is not null)
            {
                extensionsProp.ShouldSerialize = ((obj, _) =>
                {
                    var document = (T)obj;
                    return document.Extensions?.Value?.Any() ?? false;
                });
                extensionsProp.CustomConverter = new GenericSingleOrListConverter<APLExtension>(true);
            }
            var onConfigChangeProp = info.Properties.FirstOrDefault(p => p.Name == "onConfigChange");
            if (onConfigChangeProp is not null)
            {
                onConfigChangeProp.CustomConverter = new APLCommandListConverter(true);
            }
            var onMountProp = info.Properties.FirstOrDefault(p => p.Name == "onMount");
            if (onMountProp is not null)
            {
                onMountProp.CustomConverter = new APLCommandListConverter(true);
            }
        });
    }
}