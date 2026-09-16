using AlexaVoxCraft.Lambda.Abstractions;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;

namespace AlexaVoxCraft.NativeAot.TestFixture;

/// <summary>
/// Consumer-owned handler deliberately kept outside the rooted AOT test assembly. It has no
/// parameterless constructor, so Native AOT must preserve this public constructor through
/// AddAlexaSkillHost's public-constructor DAM contract.
/// </summary>
public sealed class TestHostHandler(ISkillMediator mediator, HostActivationProbe probe)
    : ILambdaHandler<SkillRequest, SkillResponse>
{
    public async Task<SkillResponse> HandleAsync(
        SkillRequest request,
        ILambdaContext context,
        CancellationToken cancellationToken)
    {
        probe.WasUsed = true;
        return await mediator.Send(request, cancellationToken);
    }
}

public sealed class HostActivationProbe
{
    public bool WasUsed { get; set; }
}
