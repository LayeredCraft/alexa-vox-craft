using AlexaVoxCraft.MediatR.Lambda.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlexaVoxCraft.MediatR.Lambda.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSkillContextAccessor()
        {
            ArgumentNullException.ThrowIfNull(services);
            
            services.TryAddSingleton<ISkillContextFactory, DefaultSkillContextFactory>();
            services.TryAddSingleton<ISkillContextAccessor, SkillContextAccessor>();
            services.TryAddScoped<SkillRequestFactory>(p =>
                () => p.GetRequiredService<ISkillContextAccessor>().SkillContext?.Request);
            return services;
        }
    }
}