using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.DI;

/// <summary>
/// Global registry that consumer assemblies "push" into at module initialization time.
/// The source generator emits a ModuleInitializer in the consumer assembly that calls <see cref="Register"/>.
/// </summary>
public static class AlexaVoxCraftRegistrar
{
    private static readonly object _gate = new();
    private static readonly List<Func<IServiceCollection, IServiceCollection>> _registrars = [];

    /// <summary>
    /// Called by generated code to provide a per-assembly DI registration action.
    /// </summary>
    public static void Register(Func<IServiceCollection, IServiceCollection> add)
    {
        ArgumentNullException.ThrowIfNull(add);
        lock (_gate)
        {
            _registrars.Add(add);
        }
    }

    /// <summary>
    /// Applies all registered DI actions; returns true if any were applied.
    /// </summary>
    public static bool TryApply(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        List<Func<IServiceCollection, IServiceCollection>> snapshot;
        lock (_gate)
        {
            snapshot = [.. _registrars];
        }

        if (snapshot.Count == 0)
            return false;

        foreach (var r in snapshot)
        {
            _ = r(services);
        }
        return true;
    }
}