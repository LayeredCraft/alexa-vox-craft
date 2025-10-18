using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace AlexaVoxCraft.MediatR.Generators.Models;

internal sealed class RegistrationModel
{
    public List<HandlerRegistration> Handlers { get; } = new();
    public HandlerRegistration? DefaultHandler { get; set; }
    public List<BehaviorRegistration> Behaviors { get; } = new();
    public List<TypeRegistration> ExceptionHandlers { get; } = new();
    public List<TypeRegistration> RequestInterceptors { get; } = new();
    public List<TypeRegistration> ResponseInterceptors { get; } = new();
    public TypeRegistration? PersistenceAdapter { get; set; }
}

internal sealed class HandlerRegistration
{
    public HandlerRegistration(INamedTypeSymbol type, INamedTypeSymbol? requestType, int lifetime, int order, Location location)
    {
        Type = type;
        RequestType = requestType;
        Lifetime = lifetime;
        Order = order;
        Location = location;
    }

    public INamedTypeSymbol Type { get; }
    public INamedTypeSymbol? RequestType { get; }
    public int Lifetime { get; }
    public int Order { get; }
    public Location Location { get; }
}

internal sealed class BehaviorRegistration
{
    public BehaviorRegistration(INamedTypeSymbol type, int lifetime, int order, Location location)
    {
        Type = type;
        Lifetime = lifetime;
        Order = order;
        Location = location;
    }

    public INamedTypeSymbol Type { get; }
    public int Lifetime { get; }
    public int Order { get; }
    public Location Location { get; }
}

internal sealed class TypeRegistration
{
    public TypeRegistration(INamedTypeSymbol type, int lifetime, Location location)
    {
        Type = type;
        Lifetime = lifetime;
        Location = location;
    }

    public INamedTypeSymbol Type { get; }
    public int Lifetime { get; }
    public Location Location { get; }
}