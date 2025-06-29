using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public class AskForPermissionPayload
{
    public AskForPermissionPayload()
    {

    }

    public AskForPermissionPayload(string permissionScope)
    {
        PermissionScope = permissionScope;
    }

    [JsonPropertyName("@type")] 
    public string Type { get; set; } = "AskForPermissionsConsentRequest";

    [JsonPropertyName("@version")] 
    public string Version { get; set; } = "1";

    [JsonPropertyName("permissionScope")]
    public string PermissionScope { get; set; }
}