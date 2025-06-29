using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.MediatR.Lambda.Context;

public abstract class SkillContext
{
    public abstract SkillRequest Request { get; }
}