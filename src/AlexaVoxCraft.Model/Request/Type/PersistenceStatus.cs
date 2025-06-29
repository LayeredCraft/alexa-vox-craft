using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public enum PersistenceStatus
{
    [EnumMember(Value= "PERSISTED")]
    Persisted,
    [EnumMember(Value= "NOT_PERSISTED")]
    NotPersisted
}