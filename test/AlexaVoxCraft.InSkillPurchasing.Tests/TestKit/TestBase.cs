using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Models;

public abstract class TestBase<TDerived>
{
    protected static JsonSerializerOptions ClientOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    protected static string Fx(string relativePath) => FromExamples(typeof(TDerived).Assembly, relativePath);

    private static string FromExamples(Assembly assembly, string relativePath)
    {
        var assemblyName = assembly.GetName().Name!;
        var tail = relativePath.Replace('\\', '/').Replace('/', '.');
        var resource = $"{assemblyName}.Examples.{tail}";
        using var stream = assembly.GetManifestResourceStream(resource)
            ?? throw new InvalidOperationException($"Missing embedded resource: {resource}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
