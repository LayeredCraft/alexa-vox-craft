using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.ConnectionTasks.Inputs;

public enum PinConfirmationStatus
{
    [EnumMember(Value = "ACHIEVED")] Achieved,
    [EnumMember(Value = "NOT_ACHIEVED")] NotAchieved,
    [EnumMember(Value = "NOT_ENABLED")] NotEnabled
}