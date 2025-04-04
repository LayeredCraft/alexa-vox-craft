using System.Reflection;
using AlexaVoxCraft.Model.Apl;
using Xunit.Abstractions;
using Xunit.Sdk;
[assembly: Xunit.TestFramework("Alexa.NET.APL.Tests.GlobalTestFramework", "Alexa.NET.APL.Tests")]

namespace Alexa.NET.APL.Tests;

public class GlobalTestFramework : ITestFramework
{
    private readonly XunitTestFramework _framework;

    public GlobalTestFramework(IMessageSink messageSink)
    {
        // ✅ Your global setup logic here
        GlobalTestInitializer.EnsureInitialized(); // 👈 Your static setup
        
        // ✅ Delegate to built-in framework
        _framework = new XunitTestFramework(messageSink);
    }

    public ISourceInformationProvider SourceInformationProvider
    {
        get => _framework.SourceInformationProvider;
        set => _framework.SourceInformationProvider = value;
    }

    public ITestFrameworkDiscoverer GetDiscoverer(IAssemblyInfo assembly) => _framework.GetDiscoverer(assembly);
    public ITestFrameworkExecutor GetExecutor(AssemblyName assemblyName) => _framework.GetExecutor(assemblyName);

    public void Dispose() => _framework.Dispose();
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