using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.TestKit;

public abstract class TestBase<TDerived>
{
    protected static JsonSerializerOptions AlexaJson = AlexaJsonOptions.DefaultOptions;
    protected static JsonSerializerOptions ClientOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
    protected static string Fx(string relativePath) => FixtureLoader.FromExamples(typeof(TDerived).Assembly, relativePath);
}