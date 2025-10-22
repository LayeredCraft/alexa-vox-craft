using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AlexaVoxCraft.MediatR.Generators.Models;
using Microsoft.CodeAnalysis;

namespace AlexaVoxCraft.MediatR.Generators.Generators;

internal static class SymbolDiscovery
{
    private const string IRequestHandlerName = "AlexaVoxCraft.MediatR.IRequestHandler";
    private const string IDefaultRequestHandlerName = "AlexaVoxCraft.MediatR.IDefaultRequestHandler";
    private const string IPipelineBehaviorName = "AlexaVoxCraft.MediatR.Pipeline.IPipelineBehavior";
    private const string IExceptionHandlerName = "AlexaVoxCraft.MediatR.Pipeline.IExceptionHandler";
    private const string IRequestInterceptorName = "AlexaVoxCraft.MediatR.Pipeline.IRequestInterceptor";
    private const string IResponseInterceptorName = "AlexaVoxCraft.MediatR.Pipeline.IResponseInterceptor";
    private const string IPersistenceAdapterName = "AlexaVoxCraft.MediatR.Attributes.Persistence.IPersistenceAdapter";

    public static ModelWithDiagnostics BuildModel(ImmutableArray<DiscoveredTypeInfo> types)
    {
        var handlers = new List<HandlerRegistration>();
        HandlerRegistration? defaultHandler = null;
        var behaviors = new List<BehaviorRegistration>();
        var exceptionHandlers = new List<TypeRegistration>();
        var requestInterceptors = new List<TypeRegistration>();
        var responseInterceptors = new List<TypeRegistration>();
        TypeRegistration? persistenceAdapter = null;

        var diagnostics = new List<DiagnosticInfo>();
        var defaultHandlerLocations = new List<Location>();
        var persistenceAdapterLocations = new List<Location>();

        foreach (var typeInfo in types)
        {
            if (typeInfo.IsAbstract || typeInfo.TypeKind != TypeKind.Class)
                continue;

            if (typeInfo.IsGenericTypeDefinition)
                continue;

            if (typeInfo.AlexaHandlerAttribute?.Exclude == true)
                continue;

            var lifetime = typeInfo.AlexaHandlerAttribute?.Lifetime ?? 0;
            var order = typeInfo.AlexaHandlerAttribute?.Order ?? 0;
            var typeModel = new Models.TypeInfo(typeInfo.FullyQualifiedTypeName);

            // Check for IDefaultRequestHandler
            if (ImplementsInterface(typeInfo, IDefaultRequestHandlerName))
            {
                defaultHandlerLocations.Add(typeInfo.Location);
                defaultHandler = new HandlerRegistration(typeModel, null, lifetime, order);
            }

            // Check for IRequestHandler<T> - can implement multiple
            var requestTypes = GetAllGenericInterfaceTypeArguments(typeInfo, IRequestHandlerName).ToList();
            foreach (var requestTypeName in requestTypes)
            {
                var requestTypeInfo = new Models.TypeInfo(requestTypeName);
                handlers.Add(new HandlerRegistration(typeModel, requestTypeInfo, lifetime, order));
            }

            // Check for IPipelineBehavior
            if (ImplementsInterface(typeInfo, IPipelineBehaviorName))
            {
                behaviors.Add(new BehaviorRegistration(typeModel, lifetime, order));
            }

            // Check for IExceptionHandler
            if (ImplementsInterface(typeInfo, IExceptionHandlerName))
            {
                exceptionHandlers.Add(new TypeRegistration(typeModel, lifetime));
            }

            // Check for IRequestInterceptor
            if (ImplementsInterface(typeInfo, IRequestInterceptorName))
            {
                requestInterceptors.Add(new TypeRegistration(typeModel, lifetime));
            }

            // Check for IResponseInterceptor
            if (ImplementsInterface(typeInfo, IResponseInterceptorName))
            {
                responseInterceptors.Add(new TypeRegistration(typeModel, lifetime));
            }

            // Check for IPersistenceAdapter
            if (ImplementsInterface(typeInfo, IPersistenceAdapterName))
            {
                persistenceAdapterLocations.Add(typeInfo.Location);
                persistenceAdapter = new TypeRegistration(typeModel, 2); // 2 = Singleton
            }
        }

        SortRegistrations(handlers, behaviors);

        // Check for multiple default handlers
        if (defaultHandlerLocations.Count > 1)
        {
            foreach (var location in defaultHandlerLocations)
            {
                diagnostics.Add(new DiagnosticInfo(DiagnosticDescriptors.MultipleDefaultHandlers, location));
            }
        }

        // Check for multiple persistence adapters
        if (persistenceAdapterLocations.Count > 1)
        {
            foreach (var location in persistenceAdapterLocations)
            {
                diagnostics.Add(new DiagnosticInfo(DiagnosticDescriptors.MultiplePersistenceAdapters, location));
            }
        }

        // Check if no handlers found
        if (handlers.Count == 0 && defaultHandler == null)
        {
            diagnostics.Add(new DiagnosticInfo(DiagnosticDescriptors.NoHandlersFound, Location.None));
        }

        var model = new RegistrationModel(
            new EquatableArray<HandlerRegistration>(handlers),
            defaultHandler,
            new EquatableArray<BehaviorRegistration>(behaviors),
            new EquatableArray<TypeRegistration>(exceptionHandlers),
            new EquatableArray<TypeRegistration>(requestInterceptors),
            new EquatableArray<TypeRegistration>(responseInterceptors),
            persistenceAdapter
        );

        return new ModelWithDiagnostics(model, new EquatableArray<DiagnosticInfo>(diagnostics));
    }

    private static void SortRegistrations(List<HandlerRegistration> handlers, List<BehaviorRegistration> behaviors)
    {
        handlers.Sort((a, b) =>
        {
            var orderCompare = a.Order.CompareTo(b.Order);
            return orderCompare != 0 ? orderCompare : string.CompareOrdinal(a.Type.FullyQualifiedName, b.Type.FullyQualifiedName);
        });

        behaviors.Sort((a, b) =>
        {
            var orderCompare = a.Order.CompareTo(b.Order);
            return orderCompare != 0 ? orderCompare : string.CompareOrdinal(a.Type.FullyQualifiedName, b.Type.FullyQualifiedName);
        });
    }

    private static bool ImplementsInterface(DiscoveredTypeInfo typeInfo, string interfaceName)
    {
        return typeInfo.ImplementedInterfaces.Any(i => i == interfaceName);
    }

    private static IEnumerable<string> GetAllGenericInterfaceTypeArguments(DiscoveredTypeInfo typeInfo, string interfaceName)
    {
        var genericInterfacePrefix = interfaceName + "<";
        return typeInfo.ImplementedInterfaces
            .Where(i => i.StartsWith(genericInterfacePrefix))
            .Select(i =>
            {
                var start = i.IndexOf('<') + 1;
                var end = i.LastIndexOf('>');
                if (start > 0 && end > start)
                {
                    return i.Substring(start, end - start);
                }
                return null;
            })
            .Where(t => t != null)!;
    }
}