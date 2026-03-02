using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AlexaVoxCraft.Model.Apl.Tests;

public static class TestModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Centralize all snapshots under: <project>/Snapshots
        UseProjectRelativeDirectory("Snapshots");

        // Make Verify(object) JSON match your style
        VerifierSettings.UseStrictJson();

        VerifierSettings.ScrubEmptyLines();
        VerifierSettings.ScrubLinesWithReplace(line => line.Replace("<!--!-->", ""));
        APLSupport.Add();

        VerifierSettings.AddExtraSettings(settings =>
            settings.Converters.Add(new JsonElementConverter()));
    }
}

file sealed class JsonElementConverter : Argon.JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(JsonElement);

    public override object? ReadJson(Argon.JsonReader reader, Type objectType, object? existingValue, Argon.JsonSerializer serializer)
        => throw new NotSupportedException();

    public override void WriteJson(Argon.JsonWriter writer, object? value, Argon.JsonSerializer serializer)
    {
        if (value is JsonElement element)
            writer.WriteRawValue(element.GetRawText());
        else
            writer.WriteNull();
    }
}