using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public enum ClearBehavior
{
    [EnumMember(Value = "CLEAR_ENQUEUED")]
    ClearEnqueued,
    [EnumMember(Value = "CLEAR_ALL")]
    ClearAll
}