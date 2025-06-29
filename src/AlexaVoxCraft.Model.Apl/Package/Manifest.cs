using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class Manifest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("installStageChanges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InstallStateChanges? InstallStageChanges { get; set; }

    [JsonPropertyName("appliesTo")]
    public string AppliesTo { get; set; }

    [JsonPropertyName("presentationDefinitions")]
    public List<PresentationDefinitionFile> PresentationDefinitions { get; set; }
}