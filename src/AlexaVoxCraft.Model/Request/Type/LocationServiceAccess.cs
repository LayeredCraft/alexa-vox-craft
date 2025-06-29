using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public enum LocationServiceAccess
{
    [EnumMember(Value = "ENABLED")]
    Enabled,
    [EnumMember(Value = "DISABLED")]
    Disabled
}