using System.Reflection;
using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.Attributes.Persistence;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.MediatR.Pipeline;
using AlexaVoxCraft.MediatR.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AlexaVoxCraft.MediatR.Registration;

public static class ServiceRegistrar
{
    public static void AddSkillMediatorClasses(this IServiceCollection services, SkillServiceConfiguration settings)
    {
        var assembliesToScan = settings.AssembliesToRegister.Distinct().ToArray();

        services.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>), assembliesToScan, true);
        
        // Single-pass enumeration with grouped filtering to minimize memory usage
        foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes).Where(t => t.IsConcrete()))
        {
            // Register default handlers
            if (type.CanBeCastTo(typeof(IDefaultRequestHandler)))
            {
                services.TryAddTransient(typeof(IDefaultRequestHandler), type);
            }
            
            // Register persistence adapters
            if (type.CanBeCastTo(typeof(IPersistenceAdapter)))
            {
                services.TryAddSingleton(typeof(IPersistenceAdapter), type);
            }
            
            // Register pipeline behaviors
            if (type.CanBeCastTo(typeof(IExceptionHandler)))
            {
                services.AddTransient(typeof(IExceptionHandler), type);
            }
            if (type.CanBeCastTo(typeof(IRequestInterceptor)))
            {
                services.AddTransient(typeof(IRequestInterceptor), type);
            }
            if (type.CanBeCastTo(typeof(IResponseInterceptor)))
            {
                services.AddTransient(typeof(IResponseInterceptor), type);
            }
        }
    }
    
    private static void ConnectImplementationsToTypesClosing(this IServiceCollection services,
        Type openRequestInterface, IEnumerable<Assembly> assembliesToScan, bool addIfAlreadyExists)
    {
        var concretions = new List<Type>();
        var interfaces = new List<Type>();
        
        foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes).Where(t => !t.IsOpenGeneric()))
        {
            var interfaceTypes = type.FindInterfacesThatClose(openRequestInterface);
            var hasInterfaces = false;
            
            foreach (var interfaceType in interfaceTypes)
            {
                hasInterfaces = true;
                interfaces.Fill(interfaceType);
            }

            if (hasInterfaces && type.IsConcrete())
            {
                concretions.Add(type);
            }
        }

        foreach (var @interface in interfaces)
        {
            if (addIfAlreadyExists)
            {
                foreach (var type in concretions.Where(x => x.CanBeCastTo(@interface)))
                {
                    services.AddTransient(@interface, type);
                }
            }
            else
            {
                var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToArray();
                if (exactMatches.Length > 1)
                {
                    exactMatches = exactMatches.Where(m => IsMatchingWithInterface(m, @interface)).ToArray();
                }

                foreach (var type in exactMatches)
                {
                    services.TryAddTransient(@interface, type);
                }
            }

            if (!@interface.IsOpenGeneric())
                services.AddConcretionsThatCouldBeClosed(@interface, concretions);
        }
    }
    private static bool IsMatchingWithInterface(Type? handlerType, Type? handlerInterface)
    {
        if (handlerType is null || handlerInterface is null)
        {
            return false;
        }

        if (handlerType.IsInterface)
        {
            if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
            {
                return true;
            }
        }
        else
        {
            return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
        }

        return false;
    }

    private static void AddConcretionsThatCouldBeClosed(this IServiceCollection services, Type @interface,
        List<Type> concretions)
    {
        foreach (var type in concretions.Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
        {
            try
            {
                services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
    {
        var openInterface = closedInterface.GetGenericTypeDefinition();
        var arguments = closedInterface.GenericTypeArguments;

        var concreteArguments = openConcretion.GenericTypeArguments;
        return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
    }

    private static bool CanBeCastTo(this Type? pluggedType, Type pluginType)
    {
        if (pluggedType is null) return false;

        return pluggedType == pluginType || pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo());
    }
    
    private static bool IsOpenGeneric(this Type type) =>
        type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;

    private static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType) =>
        FindInterfacesThatCloseCore(pluggedType, templateType).Distinct();

    private static IEnumerable<Type> FindInterfacesThatCloseCore(Type? pluggedType, Type templateType)
    {
        if (pluggedType is null) yield break;

        if (!pluggedType.IsConcrete()) yield break;

        if (templateType.GetTypeInfo().IsInterface)
        {
            foreach (var interfaceType in pluggedType.GetInterfaces().Where(type =>
                         type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
            {
                yield return interfaceType;
            }
        }
        else if (pluggedType.GetTypeInfo().BaseType!.GetTypeInfo().IsGenericType &&
                 (pluggedType.GetTypeInfo().BaseType!.GetGenericTypeDefinition() == templateType))
        {
            yield return pluggedType.GetTypeInfo().BaseType!;
        }

        if (pluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

        foreach (var interfaceType in FindInterfacesThatCloseCore(pluggedType.GetTypeInfo().BaseType, templateType))
        {
            yield return interfaceType;
        }
    }

    private static bool IsConcrete(this Type? type) =>
        !(type ?? throw new ArgumentNullException(nameof(type))).GetTypeInfo().IsAbstract &&
        !type.GetTypeInfo().IsInterface;

    private static void Fill<T>(this IList<T> list, T value)
    {
        if (list.Contains(value)) return;
        list.Add(value);
    }
    public static void AddRequiredServices(this IServiceCollection services, SkillServiceConfiguration settings)
    {
        services.TryAdd(new ServiceDescriptor(typeof(ISkillMediator), typeof(SkillMediator),
            ServiceLifetime.Transient));
        services.TryAddTransient<IHandlerInput, DefaultHandlerInput>();
        services.TryAddScoped<IAttributesManager, AttributesManager>();
        services.TryAddScoped<IResponseBuilder, DefaultResponseBuilder>();
        services.TryAddTransientExact(typeof(IPipelineBehavior), typeof(PerformanceLoggingBehavior));
        services.TryAddTransientExact(typeof(IPipelineBehavior), typeof(RequestInterceptorBehavior));
        services.TryAddTransientExact(typeof(IPipelineBehavior), typeof(ResponseInterceptorBehavior));
        services.TryAddTransientExact(typeof(IPipelineBehavior), typeof(RequestExceptionProcessBehavior));
    }

    private static void TryAddTransientExact(this IServiceCollection services, Type serviceType,
        Type implementationType)
    {
        if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType))
            return;

        services.AddTransient(serviceType, implementationType);
    }
}