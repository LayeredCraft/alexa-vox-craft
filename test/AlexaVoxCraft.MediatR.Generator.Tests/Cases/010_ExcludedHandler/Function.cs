using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Lambda;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using Microsoft.Extensions.Hosting;

namespace Sample.Generated.Function;

/// <summary>
/// Test case for source generator with a handler marked with Exclude = true.
/// Verifies that the generator properly excludes handlers from automatic registration when requested.
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