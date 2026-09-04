using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Package;

public class Manifest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("version")]
    public string Version { get; set; } = null!;

    [JsonPropertyName("installStageChanges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InstallStateChanges? InstallStageChanges { get; set; }

    [JsonPropertyName("appliesTo")]
    public string AppliesTo { get; set; } = null!;

    [JsonPropertyName("presentationDefinitions")]
    public List<PresentationDefinitionFile> PresentationDefinitions { get; set; } = null!;
}