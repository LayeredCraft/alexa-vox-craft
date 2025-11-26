using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Tests;

public abstract class TestBase
{
    protected static JsonSerializerOptions AlexaJson = AlexaJsonOptions.DefaultOptions;
    protected static string Fx(string relativePath) => FixtureLoader.FromExamples(relativePath);
}