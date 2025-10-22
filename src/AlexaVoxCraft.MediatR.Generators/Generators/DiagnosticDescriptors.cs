using Microsoft.CodeAnalysis;

namespace AlexaVoxCraft.MediatR.Generators.Generators;

internal static class DiagnosticDescriptors
{
    private const string Category = "AlexaVoxCraftGenerator";

    public static readonly DiagnosticDescriptor MultipleDefaultHandlers = new(
        id: "AVXC001",
        title: "Multiple default request handlers found",
        messageFormat: "Found multiple implementations of IDefaultRequestHandler. Only one default handler is allowed.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Only a single IDefaultRequestHandler should be registered in the application.");

    public static readonly DiagnosticDescriptor MultiplePersistenceAdapters = new(
        id: "AVXC002",
        title: "Multiple persistence adapters found",
        messageFormat: "Found multiple implementations of IPersistenceAdapter. Only one persistence adapter is allowed.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Only a single IPersistenceAdapter should be registered in the application.");

    public static readonly DiagnosticDescriptor NoHandlersFound = new(
        id: "AVXC003",
        title: "No request handlers found",
        messageFormat: "No implementations of IRequestHandler or IDefaultRequestHandler were found in the project.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "At least one request handler should be defined to handle Alexa skill requests.");
}