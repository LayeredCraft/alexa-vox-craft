using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLTDocument : APLDocumentBase
{
    public const string DocumentType = "APLT";
    [JsonPropertyName("type")]
    public override string Type => DocumentType;

    public APLTDocument():base(APLDocumentVersion.V1)
    {

    }
    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<APLTDocument>(info =>
        {
            var extensionsProp = info.Properties.FirstOrDefault(p => p.Name == "extensions");
            if (extensionsProp is not null)
            {
                extensionsProp.ShouldSerialize = ((obj, _) =>
                {
                    var document = (APLTDocument)obj;
                    return document.Extensions?.Any() ?? false;
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