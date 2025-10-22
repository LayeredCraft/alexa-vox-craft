using System;
using Microsoft.CodeAnalysis;

namespace AlexaVoxCraft.MediatR.Generators.Models;

internal readonly record struct RegistrationModel(
    EquatableArray<HandlerRegistration> Handlers,
    HandlerRegistration? DefaultHandler,
    EquatableArray<BehaviorRegistration> Behaviors,
    EquatableArray<TypeRegistration> ExceptionHandlers,
    EquatableArray<TypeRegistration> RequestInterceptors,
    EquatableArray<TypeRegistration> ResponseInterceptors,
    TypeRegistration? PersistenceAdapter
);

internal readonly record struct TypeInfo(string FullyQualifiedName) : IEquatable<TypeInfo>;

internal readonly record struct HandlerRegistration(
    TypeInfo Type,
    TypeInfo? RequestType,
    int Lifetime,
    int Order
) : IEquatable<HandlerRegistration>;

internal readonly record struct BehaviorRegistration(
    TypeInfo Type,
    int Lifetime,
    int Order
) : IEquatable<BehaviorRegistration>;

internal readonly record struct TypeRegistration(
    TypeInfo Type,
    int Lifetime
) : IEquatable<TypeRegistration>;

internal readonly record struct DiagnosticInfo(
    DiagnosticDescriptor Descriptor,
    Location Location
);