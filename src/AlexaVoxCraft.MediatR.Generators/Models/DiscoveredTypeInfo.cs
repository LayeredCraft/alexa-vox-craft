using System;
using Microsoft.CodeAnalysis;

namespace AlexaVoxCraft.MediatR.Generators.Models;

internal readonly record struct DiscoveredTypeInfo(
    string FullyQualifiedTypeName,
    bool IsAbstract,
    bool IsGenericTypeDefinition,
    TypeKind TypeKind,
    AttributeInfo? AlexaHandlerAttribute,
    EquatableArray<string> ImplementedInterfaces,
    Location Location
);

internal readonly record struct AttributeInfo(
    int Lifetime,
    int Order,
    bool Exclude
) : IEquatable<AttributeInfo>;