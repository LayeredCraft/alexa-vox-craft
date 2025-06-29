using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.DI;

public class SkillServiceConfiguration
{
    public const string SectionName = "SkillConfiguration";
    public string? CustomUserAgent { get; set; }
    public string? SkillId { get; set; }

    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    
    internal List<Assembly> AssembliesToRegister { get; } = new();
    
    public SkillServiceConfiguration RegisterServicesFromAssemblyContaining<T>()
        => RegisterServicesFromAssemblyContaining(typeof(T));
    
    public SkillServiceConfiguration RegisterServicesFromAssemblyContaining(Type type)
        => RegisterServicesFromAssembly(type.Assembly);
    
    public SkillServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);

        return this;
    }
    public SkillServiceConfiguration RegisterServicesFromAssemblies(
        params Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);

        return this;
    }

}