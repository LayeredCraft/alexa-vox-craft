using System.Reflection;

namespace AlexaVoxCraft.TestKit;

public static class FixtureLoader
{    public static string FromResource(Assembly asm, string resourceName)
    {
        using var s = asm.GetManifestResourceStream(resourceName)
                      ?? throw new InvalidOperationException($"Missing embedded resource: {resourceName}");
        using var sr = new StreamReader(s);
        return sr.ReadToEnd();
    }

    public static string FromExamples(Assembly asm, string relativePath)
    {
        var asmName = asm.GetName().Name!;
        var tail = relativePath.Replace('\\', '/').Replace('/', '.');
        var resource = $"{asmName}.Examples.{tail}";
        return FromResource(asm, resource);
    }
    
}