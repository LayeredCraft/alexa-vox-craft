using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public abstract class TestBase<TDerived>
{
    // Read through a property so tests always observe the current AlexaJsonOptions instance, including
    // after WebApiGamesSupport.Add() has registered the package context during module initialization.
    protected static JsonSerializerOptions AlexaJson => AlexaJsonOptions.DefaultOptions;

    protected static string Fx(string relativePath)
    {
        var resourceName = $"{typeof(TDerived).Assembly.GetName().Name}.Examples.{relativePath.Replace('/', '.').Replace('\\', '.')}";
        using var stream = typeof(TDerived).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' was not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
