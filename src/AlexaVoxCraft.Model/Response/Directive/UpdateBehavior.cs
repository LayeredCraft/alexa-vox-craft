using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Response.Directive;

public enum UpdateBehavior
{
    [EnumMember(Value = "REPLACE")]
    Replace,
    [EnumMember(Value = "CLEAR")]
    Clear
}