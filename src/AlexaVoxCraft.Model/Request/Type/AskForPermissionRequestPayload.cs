using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Request.Type;

public class AskForPermissionRequestPayload
{
    [JsonPropertyName("permissionScope")]
    public string PermissionScope { get; set; }

    [JsonPropertyName("status"), JsonConverter(typeof(JsonStringEnumConverterWithEnumMemberAttrSupport<PermissionStatus>))]
    public PermissionStatus Status { get; set; }
}

public enum PermissionStatus
{
    [EnumMember(Value = "ACCEPTED")]
    Accepted,
    [EnumMember(Value = "DENIED")]
    Denied,
    [EnumMember(Value = "NOT_ANSWERED")]
    NotAccepted
}