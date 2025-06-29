using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;

namespace AlexaVoxCraft.MediatR;

public interface ISkillMediator
{
    Task<SkillResponse> Send(SkillRequest request, CancellationToken cancellationToken = default);
}