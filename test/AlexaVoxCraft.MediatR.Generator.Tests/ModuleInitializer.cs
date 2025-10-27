using System.Runtime.CompilerServices;

namespace AlexaVoxCraft.MediatR.Generator.Tests;

/// <summary>
/// Module initializer for the source generator test assembly.
/// Configures Verify to work correctly with source generator snapshot testing.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the Verify framework for source generator testing.
    /// This method is called automatically when the assembly is loaded.
    /// </summary>
    [ModuleInitializer]
    public static void Init() => VerifySourceGenerators.Initialize();
}
