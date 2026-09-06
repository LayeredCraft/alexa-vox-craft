using System.Text.Json;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Tests;

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
}