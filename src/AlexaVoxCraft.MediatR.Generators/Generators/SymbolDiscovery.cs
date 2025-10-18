using System;
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

    public static RegistrationModel BuildModel(ImmutableArray<INamedTypeSymbol> symbols)
    {
        var model = new RegistrationModel();

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

            if (ImplementsInterface(symbol, IDefaultRequestHandlerName))
            {
                model.DefaultHandler = new HandlerRegistration(symbol, null, lifetime, order, location);
            }
            else if (ImplementsGenericInterface(symbol, IRequestHandlerName, out var requestType))
            {
                model.Handlers.Add(new HandlerRegistration(symbol, requestType, lifetime, order, location));
            }
            else if (ImplementsInterface(symbol, IPipelineBehaviorName))
            {
                model.Behaviors.Add(new BehaviorRegistration(symbol, lifetime, order, location));
            }
            else if (ImplementsInterface(symbol, IExceptionHandlerName))
            {
                model.ExceptionHandlers.Add(new TypeRegistration(symbol, lifetime, location));
            }
            else if (ImplementsInterface(symbol, IRequestInterceptorName))
            {
                model.RequestInterceptors.Add(new TypeRegistration(symbol, lifetime, location));
            }
            else if (ImplementsInterface(symbol, IResponseInterceptorName))
            {
                model.ResponseInterceptors.Add(new TypeRegistration(symbol, lifetime, location));
            }
            else if (ImplementsInterface(symbol, IPersistenceAdapterName))
            {
                model.PersistenceAdapter = new TypeRegistration(symbol, 2, location); // 2 = Singleton
            }
        }

        SortRegistrations(model);
        return model;
    }

    private static void SortRegistrations(RegistrationModel model)
    {
        model.Handlers.Sort((a, b) =>
        {
            var orderCompare = a.Order.CompareTo(b.Order);
            return orderCompare != 0 ? orderCompare : string.CompareOrdinal(a.Type.ToDisplayString(), b.Type.ToDisplayString());
        });

        model.Behaviors.Sort((a, b) =>
        {
            var orderCompare = a.Order.CompareTo(b.Order);
            return orderCompare != 0 ? orderCompare : string.CompareOrdinal(a.Type.ToDisplayString(), b.Type.ToDisplayString());
        });
    }

    private static bool ImplementsInterface(INamedTypeSymbol symbol, string interfaceName)
    {
        return symbol.AllInterfaces.Any(i => i.ToDisplayString() == interfaceName);
    }

    private static bool ImplementsGenericInterface(INamedTypeSymbol symbol, string interfaceName, out INamedTypeSymbol? typeArgument)
    {
        typeArgument = null;
        var genericInterface = symbol.AllInterfaces.FirstOrDefault(i =>
            i.IsGenericType &&
            i.ConstructedFrom.ToDisplayString() == interfaceName + "<TRequestType>");

        if (genericInterface == null)
            return false;

        typeArgument = genericInterface.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
        return true;
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