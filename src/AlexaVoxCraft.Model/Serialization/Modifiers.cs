using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AlexaVoxCraft.Model.Serialization;

public static class Modifiers
{
    internal static void SetNumberHandlingModifier(JsonTypeInfo jsonTypeInfo)
    {
        if (jsonTypeInfo.Type == typeof(int))
        {
            jsonTypeInfo.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        }
    }
}