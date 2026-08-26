using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests;

public abstract class TestBase<TDerived>
{
    protected static JsonSerializerOptions AlexaJson = AlexaJsonOptions.DefaultOptions;
    protected static JsonSerializerOptions ClientOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    protected static string Fx(string relativePath)
    {
        var resourceName = $"{typeof(TDerived).Assembly.GetName().Name}.Examples.{relativePath.Replace('/', '.').Replace('\\', '.')}";
        using var stream = typeof(TDerived).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' was not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
