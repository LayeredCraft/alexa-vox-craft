using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Hosting;

namespace Sample.Generated.Function;

/// <summary>
/// Test case for source generator with multiple AddSkillMediator registration calls.
/// Verifies that the generator produces a single interceptor method with multiple InterceptsLocation attributes.
/// </summary>
public class Function : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration);
                services.AddSkillMediator(context.Configuration);
                services.AddSkillMediator(context.Configuration);
            });
    }
}