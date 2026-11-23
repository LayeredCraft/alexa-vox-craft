using System.Runtime.CompilerServices;
using AlexaVoxCraft.Model.WebApiGames;

namespace AlexaVoxCraft.MediatR.Tests.WebApiGames;

/// <summary>
/// Registers Web API for Games support once, before any test in this assembly runs (the same
/// package-wide registration a real skill performs at startup). A <see cref="ModuleInitializerAttribute"/>
/// is guaranteed to run exactly once with no dependency on test-execution order.
/// </summary>
internal static class TestModuleInitializer
{
    [ModuleInitializer]
    internal static void Initialize() => WebApiGamesSupport.Add();
}
