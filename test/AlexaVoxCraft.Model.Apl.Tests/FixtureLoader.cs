using System.Reflection;

namespace AlexaVoxCraft.Model.Apl.Tests;

public static class FixtureLoader
{    public static string FromResource(string resourceName)
    {
        var asm = Assembly.GetExecutingAssembly();
        using var s = asm.GetManifestResourceStream(resourceName)
                      ?? throw new InvalidOperationException($"Missing embedded resource: {resourceName}");
        using var sr = new StreamReader(s);
        return sr.ReadToEnd();
    }

    public static string FromExamples(string relativePath)
    {
        var asm = Assembly.GetExecutingAssembly();
        var asmName = asm.GetName().Name!;
        var tail = relativePath.Replace('\\', '/').Replace('/', '.');
        var resource = $"{asmName}.Examples.{tail}";
        return FromResource(resource);
    }
    
}