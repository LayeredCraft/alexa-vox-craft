namespace AlexaVoxCraft.MediatR.Lambda.Context;

public interface ISkillContextAccessor
{
    SkillContext? SkillContext { get; set; }
}