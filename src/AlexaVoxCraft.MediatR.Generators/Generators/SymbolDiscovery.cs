using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AlexaVoxCraft.MediatR.Generators.Models;
using Microsoft.CodeAnalysis;

namespace AlexaVoxCraft.MediatR.Generators.Generators;

internal static class SymbolDiscovery
{
    private const string AlexaHandlerAttributeName = "AlexaVoxCraft.MediatR.Annotations.AlexaHandlerAttribute";
    private const string IRequestHandlerName = "AlexaVoxCraft.MediatR.IRequestHandler";
    private const string IDefaultRequestHandlerName = "AlexaVoxCraft.MediatR.IDefaultRequestHandler";
    private const string IPipelineBehaviorName = "AlexaVoxCraft.MediatR.Pipeline.IPipelineBehavior";
    private const string IExceptionHandlerName = "AlexaVoxCraft.MediatR.Pipeline.IExceptionHandler";
    private const string IRequestInterceptorName = "AlexaVoxCraft.MediatR.Pipeline.IRequestInterceptor";
    private const string IResponseInterceptorName = "AlexaVoxCraft.MediatR.Pipeline.IResponseInterceptor";
    private const string IPersistenceAdapterName = "AlexaVoxCraft.MediatR.Attributes.Persistence.IPersistenceAdapter";

    public static (RegistrationModel model, ImmutableArray<DiagnosticInfo> diagnostics) BuildModel(ImmutableArray<INamedTypeSymbol> symbols)
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

        foreach (var symbol in symbols)
        {
            if (symbol.IsAbstract || symbol.TypeKind != TypeKind.Class)
                continue;

            var attribute = GetAlexaHandlerAttribute(symbol);
            if (IsExcluded(attribute))
                continue;

            var lifetime = GetLifetime(attribute);
            var order = GetOrder(attribute);
            var location = symbol.Locations.FirstOrDefault() ?? Location.None;
            var typeInfo = new Models.TypeInfo(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

            // Check for IDefaultRequestHandler
            if (ImplementsInterface(symbol, IDefaultRequestHandlerName))
            {
                defaultHandlerLocations.Add(location);
                defaultHandler = new HandlerRegistration(typeInfo, null, lifetime, order);
            }

            // Check for IRequestHandler<T> - can implement multiple
            var requestTypes = GetAllGenericInterfaceTypeArguments(symbol, IRequestHandlerName).ToList();
            foreach (var requestType in requestTypes)
            {
                var requestTypeInfo = new Models.TypeInfo(requestType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                handlers.Add(new HandlerRegistration(typeInfo, requestTypeInfo, lifetime, order));
            }

            // Check for IPipelineBehavior
            if (ImplementsInterface(symbol, IPipelineBehaviorName))
            {
                behaviors.Add(new BehaviorRegistration(typeInfo, lifetime, order));
            }

            // Check for IExceptionHandler
            if (ImplementsInterface(symbol, IExceptionHandlerName))
            {
                exceptionHandlers.Add(new TypeRegistration(typeInfo, lifetime));
            }

            // Check for IRequestInterceptor
            if (ImplementsInterface(symbol, IRequestInterceptorName))
            {
                requestInterceptors.Add(new TypeRegistration(typeInfo, lifetime));
            }

            // Check for IResponseInterceptor
            if (ImplementsInterface(symbol, IResponseInterceptorName))
            {
                responseInterceptors.Add(new TypeRegistration(typeInfo, lifetime));
            }

            // Check for IPersistenceAdapter
            if (ImplementsInterface(symbol, IPersistenceAdapterName))
            {
                persistenceAdapterLocations.Add(location);
                persistenceAdapter = new TypeRegistration(typeInfo, 2); // 2 = Singleton
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

        return (model, diagnostics.ToImmutableArray());
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

    private static bool ImplementsInterface(INamedTypeSymbol symbol, string interfaceName)
    {
        return symbol.AllInterfaces.Any(i => i.ToDisplayString() == interfaceName);
    }

    private static IEnumerable<INamedTypeSymbol> GetAllGenericInterfaceTypeArguments(INamedTypeSymbol symbol, string interfaceName)
    {
        return symbol.AllInterfaces
            .Where(i => i.IsGenericType &&
                       i.ConstructedFrom.ToDisplayString() == interfaceName + "<TRequestType>")
            .Select(i => i.TypeArguments.FirstOrDefault() as INamedTypeSymbol)
            .Where(t => t != null)!;
    }

    private static AttributeData? GetAlexaHandlerAttribute(INamedTypeSymbol symbol)
    {
        return symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == AlexaHandlerAttributeName);
    }

    private static bool IsExcluded(AttributeData? attribute)
    {
        if (attribute == null)
            return false;

        var excludeArg = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Exclude");
        return excludeArg.Value.Value is true;
    }

    private static int GetLifetime(AttributeData? attribute)
    {
        if (attribute == null)
            return 0; // 0 = Transient

        var lifetimeArg = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Lifetime");
        if (lifetimeArg.Value.Value is int lifetimeValue)
        {
            return lifetimeValue;
        }

        return 0; // 0 = Transient
    }

    private static int GetOrder(AttributeData? attribute)
    {
        if (attribute == null)
            return 0;

        var orderArg = attribute.NamedArguments.FirstOrDefault(a => a.Key == "Order");
        return orderArg.Value.Value is int orderValue ? orderValue : 0;
    }
}