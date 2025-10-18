using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.Annotations;

/// <summary>
/// Controls DI behavior and selection priority for AlexaVoxCraft handlers.
/// Apply to request handlers, interceptors, etc.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AlexaHandlerAttribute : Attribute
{
    /// <summary>
    /// Service lifetime used when registering this handler. Defaults to Transient.
    /// </summary>
    public ServiceLifetime Lifetime { get; init; } = ServiceLifetime.Transient;

    /// <summary>
    /// Determines selection priority when multiple handlers match.
    /// Lower numbers are selected first. Defaults to 0.
    /// </summary>
    /// <remarks>
    /// Deterministic tie-breaker (if two handlers share the same <see cref="Order"/>):
    /// fully-qualified type name ascending (ordinal, case-sensitive).
    /// </remarks>
    public int Order { get; init; } = 0;

    /// <summary>
    /// When true, this handler is excluded from registration/selection.
    /// </summary>
    public bool Exclude { get; init; } = false;
}