using AlexaVoxCraft.MediatR.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSkillMediator(this IServiceCollection services, IConfiguration configuration,
        Action<SkillServiceConfiguration> settingsAction, string sectionName = SkillServiceConfiguration.SectionName)
    {
        // Step 1: Bind configuration values into a new instance
        var serviceConfig = new SkillServiceConfiguration();
        configuration.GetSection(sectionName).Bind(serviceConfig);

        // Step 2: Apply user-provided settings via the settingsAction
        settingsAction.Invoke(serviceConfig);

        // Step 3: Register configuration with DI to enable IOptions<SkillServiceConfiguration>
        services.Configure<SkillServiceConfiguration>(opt =>
        {
            // Combine values from the bound configuration and the action
            configuration.GetSection(sectionName).Bind(opt);
            settingsAction.Invoke(opt);
        });

        // Step 4: Pass the combined configuration for further processing
        return services.AddSkillMediator(serviceConfig);
    }

    private static IServiceCollection AddSkillMediator(this IServiceCollection services, SkillServiceConfiguration settings)
    {
        
        if (settings.AssembliesToRegister.Count == 0)
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");

        services.AddRequiredServices(settings);
        services.AddSkillMediatorClasses(settings);
        
        return services;
    }
}