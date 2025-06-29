using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Request;

public enum SlotValueType
{
    [EnumMember(Value="Simple")]
    Simple,
    [EnumMember(Value="List")]
    List
}