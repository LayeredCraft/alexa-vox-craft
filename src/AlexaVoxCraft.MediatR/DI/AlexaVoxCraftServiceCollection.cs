using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.MediatR.DI;

/// <summary>
/// Compile-time stub. The generator implements the partial methods.
/// </summary>
internal static partial class AlexaVoxCraftServiceCollection
{
    /// <summary>
    /// True when generated registrations are available.
    /// The generator implements <see cref="GetIsAvailable"/> to return true.
    /// </summary>
    public static bool IsAvailable => GetIsAvailable();

    /// <summary>
    /// Adds generated registrations when available.
    /// The generator implements <see cref="AddGeneratedCore"/>.
    /// </summary>
    public static IServiceCollection AddAlexaVoxCraftGenerated(this IServiceCollection services)
        => AddGeneratedCore(services);

    // ---- Partial methods that the generator will implement ----

    // No body here; generator supplies the implementation
    private static partial bool GetIsAvailable();

    // No body here; generator supplies the implementation
    private static partial IServiceCollection AddGeneratedCore(this IServiceCollection services);
}