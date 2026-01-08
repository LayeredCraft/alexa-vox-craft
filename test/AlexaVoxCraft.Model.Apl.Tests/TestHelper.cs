using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests;

public static class TestHelper
{
    public static async Task VerifyRequestObject<TRequest>(TRequest request)
    {
        await Verify(request).DisableDiff();
    }

    public static async Task VerifySerializedObject<T>(T obj, JsonSerializerOptions? options = null, string? parameters = null)
    {
        var json = JsonSerializer.Serialize(obj, options ?? AlexaJsonOptions.DefaultOptions);
        var verification = Verify(json);

        if (!string.IsNullOrEmpty(parameters))
        {
            verification = verification.UseParameters(parameters);
        }

        await verification;
    }

    public static async Task VerifyDeserializedObject<T>(string json, JsonSerializerOptions? options = null, string? parameters = null)
    {
        var obj = JsonSerializer.Deserialize<T>(json, options ?? AlexaJsonOptions.DefaultOptions);
        var verification = Verify(obj);

        if (!string.IsNullOrEmpty(parameters))
        {
            verification = verification.UseParameters(parameters);
        }

        await verification;
    }
}