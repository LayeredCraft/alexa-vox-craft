using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Lambda.Context;

public sealed class DefaultSkillContext : SkillContext
{
    public DefaultSkillContext(SkillRequest request) => Request = request;

    public override SkillRequest Request { get; }
}