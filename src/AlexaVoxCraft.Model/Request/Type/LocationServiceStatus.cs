using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public enum LocationServiceStatus
{
    [EnumMember(Value = "RUNNING")]
    Running,
    [EnumMember(Value = "STOPPED")]
    Stopped
}