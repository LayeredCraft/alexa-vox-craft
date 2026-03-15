using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Hosting;

namespace Sample.Generated.Function;

/// <summary>
/// Test case for source generator with interceptors using AlexaHandlerAttribute to specify execution order.
/// Interceptor class names are intentionally prefixed with Z/A to conflict with alphabetical ordering,
/// proving that Order property—not name—controls registration sequence.
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