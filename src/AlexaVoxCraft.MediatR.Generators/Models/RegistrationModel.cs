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

internal readonly record struct TypeInfo(string FullyQualifiedName);

internal readonly record struct HandlerRegistration(
    TypeInfo Type,
    TypeInfo? RequestType,
    int Lifetime,
    int Order
);

internal readonly record struct BehaviorRegistration(
    TypeInfo Type,
    int Lifetime,
    int Order
);

internal readonly record struct TypeRegistration(
    TypeInfo Type,
    int Lifetime
);

internal readonly record struct DiagnosticInfo(
    DiagnosticDescriptor Descriptor,
    Location Location
);

internal readonly record struct ModelWithDiagnostics(
    RegistrationModel Model,
    EquatableArray<DiagnosticInfo> Diagnostics
);