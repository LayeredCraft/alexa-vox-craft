using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Hosting;

namespace Sample.Generated.Function;

/// <summary>
/// Test case for source generator with handlers and request/response interceptors.
/// Verifies that the generator properly registers handlers along with pipeline interceptors.
/// </summary>
public class Function : AlexaSkillFunction<SkillRequest, SkillResponse>
{
    protected override void Init(IHostBuilder builder)
    {
        builder
            .ConfigureServices((context, services) =>
            {
                services.AddSkillMediator(context.Configuration);
            });
    }
}