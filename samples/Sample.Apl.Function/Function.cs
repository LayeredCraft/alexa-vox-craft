using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.MediatR.Lambda.Extensions;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Hosting;

namespace Sample.Apl.Function;

public class Function : AlexaSkillFunction<APLSkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .UseHandler<LambdaHandler, APLSkillRequest, SkillResponse>()
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration,
                    cfg => { cfg.RegisterServicesFromAssemblyContaining<Program>(); });
            });
    }
}