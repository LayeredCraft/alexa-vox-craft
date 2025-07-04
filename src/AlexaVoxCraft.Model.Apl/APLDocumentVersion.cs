﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;

namespace AlexaVoxCraft.Model.Apl;

[JsonConverter(typeof(UnknownDocumentVersionConverter))]
public enum APLDocumentVersion
{
    Unknown,
    [EnumMember(Value = "1.0")]
    V1,
    [EnumMember(Value = "1.1")]
    V1_1,
    [EnumMember(Value = "1.2")]
    V1_2,
    [EnumMember(Value = "1.3")]
    V1_3,
    [EnumMember(Value = "1.4")]
    V1_4,
    [EnumMember(Value = "1.5")]
    V1_5,
    [EnumMember(Value = "1.6")]
    V1_6,
    [EnumMember(Value = "1.7")]
    V1_7,
    [EnumMember(Value = "1.8")]
    V1_8,
    [EnumMember(Value = "1.9")]
    V1_9,
    [EnumMember(Value = "2022.1")]
    V2022_1,
    [EnumMember(Value = "2022.2")]
    V2022_2,
    [EnumMember(Value = "2023.1")]
    V2023_1,
    [EnumMember(Value = "2023.2")]
    V2023_2,
    [EnumMember(Value = "2024.1")]
    V2024_1
}