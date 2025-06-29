using AlexaVoxCraft.MediatR.Lambda.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlexaVoxCraft.MediatR.Lambda.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSkillContextAccessor(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));
        services.TryAddSingleton<ISkillContextFactory, DefaultSkillContextFactory>();
        services.TryAddSingleton<ISkillContextAccessor, SkillContextAccessor>();
        services.TryAddScoped<SkillRequestFactory>(p =>
            () => p.GetRequiredService<ISkillContextAccessor>().SkillContext?.Request);
        return services;
    }
}