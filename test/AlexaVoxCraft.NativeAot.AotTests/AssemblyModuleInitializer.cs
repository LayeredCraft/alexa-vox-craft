using System.Runtime.CompilerServices;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.NativeAot.AotTests;

/// <summary>
/// One-time, process-wide setup the old console app performed exactly once at the top of its single
/// sequential <c>Main()</c> - <see cref="APLSupport.Add"/> (registers APL's package type-info resolver)
/// and <see cref="AlexaJsonOptions.RegisterTypeInfoResolver"/> (registers this project's own consumer
/// metadata, <see cref="ValidationAppContext"/>) are both process-global static registrations, not
/// scoped to a request or a test. A <see cref="ModuleInitializerAttribute"/> method is guaranteed to
/// run exactly once, before any other code in this assembly - unlike relying on xUnit test-execution
/// order (which independent [Theory]/[Compose] tests deliberately have none of here), this has no
/// dependency on which test happens to run first, and matches a real skill's own cold-start behavior
/// (these calls happen once at Lambda cold start, never per-request) more accurately than the original
/// app's ordering-by-coincidence did.
/// </summary>
internal static class AssemblyModuleInitializer
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        APLSupport.Add();
        AlexaJsonOptions.RegisterTypeInfoResolver(ValidationAppContext.Default);
    }
}
