using System.Collections.Generic;
using System.Text.Json;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class StringOrArrayConverter : SingleOrListConverter<string>
{
    protected override JsonTokenType SingleTokenType => JsonTokenType.String;

    public StringOrArrayConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }

    protected override void ReadSingle(ref Utf8JsonReader reader, JsonSerializerOptions options, List<string> list)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            list.Add(reader.GetString()!);
        }
        else
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }
    }
}

public class StringOrArrayValueCollectionConverter : APLValueCollectionConverter<string>
{
    public StringOrArrayValueCollectionConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }

    protected override void ReadSingle(ref Utf8JsonReader reader, JsonSerializerOptions options, IList<string> list)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            list.Add(reader.GetString()!);
        }
        else
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }

    }
}