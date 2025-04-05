﻿using System.Collections.Generic;
using System.Text.Json;
using JsonException = Newtonsoft.Json.JsonException;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class StringOrArrayConverter : SingleOrListConverter<string>
{
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
    }}