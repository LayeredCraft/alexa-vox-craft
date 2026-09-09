using System.Diagnostics.CodeAnalysis;
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
    // This is the reflection-based assembly-scanning fallback (ADR-0001: not Native-AOT-supported).
    // The generator-interceptor path (the supported, AOT-safe way to call AddSkillMediator) never calls
    // this method - only the base, non-intercepted AddSkillMediator extension does, when the interceptor
    // didn't run (pre-8.0.400 SDK or EnableMediatRGeneratorInterceptor=false). Annotated so a consumer who
    // calls this method directly - bypassing AddSkillMediator entirely - gets a compile-time warning under
    // trim/AOT instead of silently shipping a reflection path that breaks at runtime.
    [RequiresUnreferencedCode("Uses reflection-based assembly scanning to discover and register handlers. Not supported under Native AOT/trimming - use the generator-interceptor path (AddSkillMediator with the interceptor enabled) instead.")]
    [RequiresDynamicCode("Uses Type.MakeGenericType to close open generic handler implementations. Not supported under Native AOT - use the generator-interceptor path (AddSkillMediator with the interceptor enabled) instead.")]
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
                // Single enumeration with compound predicate to avoid intermediate allocation
                var candidateTypes = concretions.Where(x => x.CanBeCastTo(@interface));
                var exactMatches = candidateTypes.ToList();

                // Apply additional filtering only if multiple matches exist
                if (exactMatches.Count > 1)
                {
                    exactMatches = exactMatches.Where(m => IsMatchingWithInterface(m, @interface)).ToList();
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
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, SkillServiceConfiguration settings)
    {
        services.TryAdd(new ServiceDescriptor(typeof(ISkillMediator), typeof(SkillMediator),
            ServiceLifetime.Transient));
        services.TryAddTransient<IHandlerInput, DefaultHandlerInput>();
        services.TryAddScoped<IAttributesManager, AttributesManager>();
        services.TryAddScoped<IResponseBuilder, DefaultResponseBuilder>();
        services.TryAddTransientExact<IPipelineBehavior, PerformanceLoggingBehavior>();
        services.TryAddTransientExact<IPipelineBehavior, RequestInterceptorBehavior>();
        services.TryAddTransientExact<IPipelineBehavior, ResponseInterceptorBehavior>();
        services.TryAddTransientExact<IPipelineBehavior, RequestExceptionProcessBehavior>();

        return services;
    }

    // Compile-time-closed generic registration (not the Type,Type overload) - this is the supported
    // Native AOT path (called unconditionally by both the generator-interceptor and reflection-fallback
    // AddSkillMediator paths, per plan 0003 Task Group 6): AddTransient(Type,Type)'s
    // DynamicallyAccessedMembers-annotated Type parameters preserve constructor *metadata* for
    // trimming, but do not by themselves guarantee the AOT compiler emits native code for that
    // constructor's reflective invocation the way the generic AddTransient<TService,TImplementation>()
    // overload does - confirmed empirically via the rooted Native AOT validation app
    // (test/AlexaVoxCraft.NativeAot.ValidationApp), which failed constructing PerformanceLoggingBehavior
    // at runtime under a published native binary until this method switched to the generic overload.
    private static void TryAddTransientExact<TService,
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (services.Any(reg => reg.ServiceType == typeof(TService) && reg.ImplementationType == typeof(TImplementation)))
            return;

        services.AddTransient<TService, TImplementation>();
    }
}