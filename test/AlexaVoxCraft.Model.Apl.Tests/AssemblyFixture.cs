using System.Reflection;
using Xunit.v3;

[assembly: TestFramework(typeof(AlexaVoxCraft.Model.Apl.Tests.GlobalTestFramework))]

namespace AlexaVoxCraft.Model.Apl.Tests;

public class GlobalTestFramework : ITestFramework
{
    private readonly XunitTestFramework _framework;

    public GlobalTestFramework()
    {
        // ✅ Your global setup logic here
        GlobalTestInitializer.EnsureInitialized(); // 👈 Your static setup
        
        // ✅ Delegate to built-in framework
        _framework = new XunitTestFramework();
    }

    public string TestFrameworkDisplayName => _framework.TestFrameworkDisplayName;

    public ITestFrameworkDiscoverer GetDiscoverer(Assembly assembly) => _framework.GetDiscoverer(assembly);
    
    public ITestFrameworkExecutor GetExecutor(Assembly assembly) => _framework.GetExecutor(assembly);

    public void SetTestPipelineStartup(ITestPipelineStartup testPipelineStartup) => _framework.SetTestPipelineStartup(testPipelineStartup);
}

public static class GlobalTestInitializer
{
    private static bool _initialized;

    public static void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;

        // 🔥 Register all JSON converters/typeinfo modifiers/etc.
        APLSupport.Add();
    }
}