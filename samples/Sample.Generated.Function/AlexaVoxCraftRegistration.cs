using AlexaVoxCraft.MediatR.Annotations;
using AlexaVoxCraft.MediatR.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Generated.Function;

[AlexaVoxCraftRegistration]
public static partial class AlexaVoxCraftRegistration
{
    public static partial IServiceCollection AddAlexaSkillMediator(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SkillServiceConfiguration>? settingsAction = null,
        string sectionName = SkillServiceConfiguration.SectionName);
}