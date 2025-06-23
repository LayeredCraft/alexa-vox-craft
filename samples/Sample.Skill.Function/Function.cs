using System.Diagnostics.CodeAnalysis;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.MediatR.Lambda.Extensions;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Hosting;

namespace Sample.Skill.Function;

[ExcludeFromCodeCoverage]
public class Function : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, SkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration,
                    cfg => { cfg.RegisterServicesFromAssemblyContaining<Program>(); });
            });
    }
}