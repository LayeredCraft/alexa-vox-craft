﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Apl.VectorGraphics;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl;

public class APLDocument: APLDocumentBase
{
    public const string DocumentType = "APL";
    [JsonPropertyName("type")]
    public override string Type => DocumentType;

    public APLDocument()
    {
            
    }

    public APLDocument(APLDocumentVersion version) : base(version)
    {

    }

    [JsonPropertyName("handleKeyDown")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLKeyboardHandler>>? HandleKeyDown { get; set; }

    [JsonPropertyName("handleKeyUp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLKeyboardHandler>>? HandleKeyUp { get; set; }

    [JsonPropertyName("theme")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ViewportTheme? Theme { get; set; }

    [JsonPropertyName("import")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<Import>? Imports { get; set; }

    [JsonPropertyName("styles")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, Style>? Styles { get; set; }

    [JsonPropertyName("graphics")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string,AVG>? Graphics { get; set; }

    [JsonPropertyName("commands")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, CommandDefinition>? Commands { get; set; }

    [JsonPropertyName("export")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExportList? Export { get; set; }

    [JsonPropertyName("background")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<DocumentBackgroundColor>? Background { get; set; }

    [JsonPropertyName("onDisplayStateChange")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<IList<APLCommand>> OnDisplayStateChange { get; set; }
    public static void AddSupport()
    {
        AlexaJsonOptions.RegisterTypeModifier<APLDocument>(info =>
        {
            var onDisplayStateChangeProp = info.Properties.FirstOrDefault(p => p.Name == "onDisplayStateChange");
            if (onDisplayStateChangeProp is not null)
            {
                onDisplayStateChangeProp.CustomConverter = new APLCommandListConverter(true);
            }
            var handleKeyUpProp = info.Properties.FirstOrDefault(p => p.Name == "handleKeyUp");
            if (handleKeyUpProp is not null)
            {
                handleKeyUpProp.CustomConverter = new APLKeyboardHandlerConverter(false);
            }
            var handleKeyDownProp = info.Properties.FirstOrDefault(p => p.Name == "handleKeyDown");
            if (handleKeyDownProp is not null)
            {
                handleKeyDownProp.CustomConverter = new APLKeyboardHandlerConverter(false);
            }
        });
    }
}