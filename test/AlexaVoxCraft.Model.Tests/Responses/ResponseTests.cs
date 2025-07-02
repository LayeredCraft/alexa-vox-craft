using System.Text.Json;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Directive.Templates;
using AlexaVoxCraft.Model.Response.Ssml;
using AlexaVoxCraft.Model.Tests.Infrastructure;

namespace AlexaVoxCraft.Model.Tests.Responses;

public sealed class ResponseTests
{
    private const string ExamplesPath = @"Examples";

    [Fact]
    public void Should_create_same_json_response_as_example()
    {
        var skillResponse = new SkillResponse
        {
            Version = "1.0",
            SessionAttributes = new Dictionary<string, object>
            {
                {
                    "supportedHoriscopePeriods", new
                    {
                        daily = true,
                        weekly = false,
                        monthly = false
                    }
                }
            },
            Response = new ResponseBody
            {
                OutputSpeech = new PlainTextOutputSpeech
                {
                    Text =
                        "Today will provide you a new learning opportunity. Stick with it and the possibilities will be endless. Can I help you with anything else?"
                },
                Card = new SimpleCard
                {
                    Title = "Horoscope",
                    Content =
                        "Today will provide you a new learning opportunity. Stick with it and the possibilities will be endless."
                },
                ShouldEndSession = false
            }
        };

        Assert.True(Utility.CompareJson(skillResponse, "Response.json"));
    }


    [Fact]
    public void Create_HintDirective()
    {
        // Arrange: create the actual object
        var actual = new HintDirective
        {
            Hint = new Hint
            {
                Text = "sample text",
                Type = TextType.Plain
            }
        };

        // Arrange: define the expected JSON
        const string expectedJson = """
                                    {
                                        "type": "Hint",
                                        "hint": {
                                            "type": "PlainText",
                                            "text": "sample text"
                                        }
                                    }
                                    """;
        
        // Act & Assert: compare JSON structures using your deep equality method
        var actualJson = JsonSerializer.Serialize(actual);
        using var actualDoc = JsonDocument.Parse(actualJson);
        using var expectedDoc = JsonDocument.Parse(expectedJson);
        Assert.True(actualDoc.RootElement.JsonElementDeepEquals(expectedDoc.RootElement));
    }


    [Fact]
    public void RepromptStringGeneratesPlainTextOutput()
    {
        var result = new Reprompt("text");
        Assert.IsType<PlainTextOutputSpeech>(result.OutputSpeech);
        var plainText = (PlainTextOutputSpeech)result.OutputSpeech;
        Assert.Equal("text", plainText.Text);
    }

    [Fact]
    public void RepromptSsmlGeneratesPlainTextOutput()
    {
        var speech = new Response.Ssml.Speech(new PlainText("text"));
        var result = new Reprompt(speech);
        Assert.IsType<SsmlOutputSpeech>(result.OutputSpeech);
        var ssmlText = (SsmlOutputSpeech)result.OutputSpeech;
        Assert.Equal(speech.ToXml(), ssmlText.Ssml);
    }

    [Fact]
    public void DeserializesExampleSimpleCardJson()
    {
        var deserialized = Utility.ExampleFileContent<ICard>("SimpleCard.json");

        Assert.Equal(typeof(SimpleCard), deserialized.GetType());

        var card = (SimpleCard)deserialized;

        Assert.Equal("Simple", card.Type);
        Assert.Equal("Example Title", card.Title);
        Assert.Equal("Example Body Text", card.Content);
    }

    [Fact]
    public void DeserializesExampleStandardCardJson()
    {
        var deserialized = Utility.ExampleFileContent<ICard>("StandardCard.json");

        Assert.Equal(typeof(StandardCard), deserialized.GetType());

        var card = (StandardCard)deserialized;

        Assert.Equal("Standard", card.Type);
        Assert.Equal("Example Title", card.Title);
        Assert.Equal("Example Body Text", card.Content);
        Assert.Equal("https://example.com/smallImage.png", card.Image.SmallImageUrl);
        Assert.Equal("https://example.com/largeImage.png", card.Image.LargeImageUrl);
    }

    [Fact]
    public void DeserializesExampleLinkAccountCardJson()
    {
        var deserialized = Utility.ExampleFileContent<ICard>("LinkAccountCard.json");

        Assert.Equal(typeof(LinkAccountCard), deserialized.GetType());

        var card = (LinkAccountCard)deserialized;

        Assert.Equal("LinkAccount", card.Type);
    }

    [Fact]
    public void DeserializesExampleAskForPermissionsConsentJson()
    {
        var deserialized = Utility.ExampleFileContent<ICard>("AskForPermissionsConsent.json");

        Assert.Equal(typeof(AskForPermissionsConsentCard), deserialized.GetType());

        var card = (AskForPermissionsConsentCard)deserialized;

        Assert.Equal("AskForPermissionsConsent", card.Type);
        Assert.Equal(2, card.Permissions.Count);
        Assert.NotEqual("read::alexa:household:list", card.Permissions.First());
        Assert.Equal("alexa::household:lists:read", card.Permissions.First());
        Assert.Equal("alexa::household:lists:write", card.Permissions.Last());
    }

    [Fact]
    public void SerializesPlainTextOutputSpeechToJson()
    {
        var plainTextOutputSpeech = new PlainTextOutputSpeech { Text = "text content" };
        Assert.True(Utility.CompareJson(plainTextOutputSpeech, "PlainTextOutputSpeech.json"));
    }

    [Fact]
    public void DeserializesExamplePlainTextOutputSpeechJson()
    {
        var deserialized = Utility.ExampleFileContent<IOutputSpeech>("PlainTextOutputSpeech.json");

        Assert.Equal(typeof(PlainTextOutputSpeech), deserialized.GetType());

        var outputSpeech = (PlainTextOutputSpeech)deserialized;

        Assert.Equal("PlainText", outputSpeech.Type);
        Assert.Equal("text content", outputSpeech.Text);
        Assert.Null(outputSpeech.PlayBehavior);
    }

    [Fact]
    public void SerializesPlainTextOutputSpeechWithPlayBehaviorToJson()
    {
        var plainTextOutputSpeech = new PlainTextOutputSpeech { Text = "text content", PlayBehavior = PlayBehavior.ReplaceAll };
        Assert.True(Utility.CompareJson(plainTextOutputSpeech, "PlainTextOutputSpeechWithPlayBehavior.json"));
    }

    [Fact]
    public void DeserializesExamplePlainTextOutputSpeechWithPlayBehaviorJson()
    {
        var deserialized = Utility.ExampleFileContent<IOutputSpeech>("PlainTextOutputSpeechWithPlayBehavior.json");

        Assert.Equal(typeof(PlainTextOutputSpeech), deserialized.GetType());

        var outputSpeech = (PlainTextOutputSpeech)deserialized;

        Assert.Equal("PlainText", outputSpeech.Type);
        Assert.Equal("text content", outputSpeech.Text);
        Assert.Equal(PlayBehavior.ReplaceAll, outputSpeech.PlayBehavior);
    }

    [Fact]
    public void SerializesSsmlOutputSpeechToJson()
    {
        var ssmlOutputSpeech = new SsmlOutputSpeech { Ssml = "ssml content" };
        Assert.True(Utility.CompareJson(ssmlOutputSpeech, "SsmlOutputSpeech.json"));
    }

    [Fact]
    public void DeserializesExampleSsmlOutputSpeechJson()
    {
        var deserialized = Utility.ExampleFileContent<IOutputSpeech>("SsmlOutputSpeech.json");

        Assert.Equal(typeof(SsmlOutputSpeech), deserialized.GetType());

        var outputSpeech = (SsmlOutputSpeech)deserialized;

        Assert.Equal("SSML", outputSpeech.Type);
        Assert.Equal("ssml content", outputSpeech.Ssml);
        Assert.Null(outputSpeech.PlayBehavior);
    }

    [Fact]
    public void SerializesSsmlOutputSpeechWithPlayBehaviorToJson()
    {
        var ssmlOutputSpeech = new SsmlOutputSpeech { Ssml = "ssml content", PlayBehavior = PlayBehavior.ReplaceEnqueued };
        Assert.True(Utility.CompareJson(ssmlOutputSpeech, "SsmlOutputSpeechWithPlayBehavior.json"));
    }

    [Fact]
    public void DeserializesExampleSsmlOutputSpeechWithPlayBehaviorJson()
    {
        var deserialized = Utility.ExampleFileContent<IOutputSpeech>("SsmlOutputSpeechWithPlayBehavior.json");

        Assert.Equal(typeof(SsmlOutputSpeech), deserialized.GetType());

        var outputSpeech = (SsmlOutputSpeech)deserialized;

        Assert.Equal("SSML", outputSpeech.Type);
        Assert.Equal("ssml content", outputSpeech.Ssml);
        Assert.Equal(PlayBehavior.ReplaceEnqueued, outputSpeech.PlayBehavior);
    }


    [Fact]
    public void DeserializesExampleDialogConfirmIntentJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("DialogConfirmIntent.json");

        Assert.Equal(typeof(DialogConfirmIntent), deserialized.GetType());

        var directive = (DialogConfirmIntent)deserialized;

        Assert.Equal("Dialog.ConfirmIntent", directive.Type);
        Assert.Equal("GetZodiacHoroscopeIntent", directive.UpdatedIntent.Name);
        Assert.Equal(ConfirmationStatus.None, directive.UpdatedIntent.ConfirmationStatus);

        var slot1 = directive.UpdatedIntent.Slots["ZodiacSign"];

        Assert.Equal("ZodiacSign", slot1.Name);
        Assert.Equal("virgo", slot1.Value);
        Assert.Equal(ConfirmationStatus.Confirmed, slot1.ConfirmationStatus);

        var slot2 = directive.UpdatedIntent.Slots["Date"];

        Assert.Equal("Date", slot2.Name);
        Assert.Equal("2015-11-25", slot2.Value);
        Assert.Equal(ConfirmationStatus.Confirmed, slot2.ConfirmationStatus);
    }

    [Fact]
    public void DeserializesExampleDialogConfirmSlotJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("DialogConfirmSlot.json");

        Assert.Equal(typeof(DialogConfirmSlot), deserialized.GetType());

        var directive = (DialogConfirmSlot)deserialized;

        Assert.Equal("Dialog.ConfirmSlot", directive.Type);
        Assert.Equal("Date", directive.SlotName);
        Assert.Equal("GetZodiacHoroscopeIntent", directive.UpdatedIntent.Name);

        var slot1 = directive.UpdatedIntent.Slots["ZodiacSign"];

        Assert.Equal("ZodiacSign", slot1.Name);
        Assert.Equal("virgo", slot1.Value);

        var slot2 = directive.UpdatedIntent.Slots["Date"];

        Assert.Equal("Date", slot2.Name);
        Assert.Equal("2015-11-25", slot2.Value);
        Assert.Equal(ConfirmationStatus.Confirmed, slot2.ConfirmationStatus);
    }

    [Fact]
    public void DeserializesExampleDialogDelegateJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("DialogDelegate.json");

        Assert.Equal(typeof(DialogDelegate), deserialized.GetType());

        var directive = (DialogDelegate)deserialized;

        Assert.Equal("Dialog.Delegate", directive.Type);
        Assert.Equal("GetZodiacHoroscopeIntent", directive.UpdatedIntent.Name);
        Assert.Equal(ConfirmationStatus.None, directive.UpdatedIntent.ConfirmationStatus);

        var slot1 = directive.UpdatedIntent.Slots["ZodiacSign"];

        Assert.Equal("ZodiacSign", slot1.Name);
        Assert.Equal("virgo", slot1.Value);

        var slot2 = directive.UpdatedIntent.Slots["Date"];

        Assert.Equal("Date", slot2.Name);
        Assert.Equal("2015-11-25", slot2.Value);
        Assert.Equal(ConfirmationStatus.Confirmed, slot2.ConfirmationStatus);
    }

    [Fact]
    public void DeserializesExampleDialogElicitSlotJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("DialogElicitSlot.json");

        Assert.Equal(typeof(DialogElicitSlot), deserialized.GetType());

        var directive = (DialogElicitSlot)deserialized;

        Assert.Equal("Dialog.ElicitSlot", directive.Type);
        Assert.Equal("ZodiacSign", directive.SlotName);
        Assert.Equal("GetZodiacHoroscopeIntent", directive.UpdatedIntent.Name);
        Assert.Equal(ConfirmationStatus.None, directive.UpdatedIntent.ConfirmationStatus);

        var slot1 = directive.UpdatedIntent.Slots["ZodiacSign"];

        Assert.Equal("ZodiacSign", slot1.Name);
        Assert.Equal("virgo", slot1.Value);

        var slot2 = directive.UpdatedIntent.Slots["Date"];

        Assert.Equal("Date", slot2.Name);
        Assert.Equal("2015-11-25", slot2.Value);
        Assert.Equal(ConfirmationStatus.Confirmed, slot2.ConfirmationStatus);
    }

    [Fact]
    public void DeserializesExampleHintDirectiveJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("HintDirective.json");

        Assert.Equal(typeof(HintDirective), deserialized.GetType());

        var directive = (HintDirective)deserialized;

        Assert.Equal("Hint", directive.Type);
        Assert.Equal("PlainText", directive.Hint.Type);
        Assert.Equal("test hint", directive.Hint.Text);
    }


    [Fact]
    public void DeserializesExampleResponseJson()
    {
        var deserialized = Utility.ExampleFileContent<SkillResponse>("Response.json");

        Assert.Equal("1.0", deserialized.Version);

        // Attempt to cast the object to a Dictionary<string, object>
        var periods = Assert.IsType<Dictionary<string, object>>(deserialized.SessionAttributes["supportedHoriscopePeriods"]);

        Assert.True(Convert.ToBoolean(periods["daily"]));
        Assert.False(Convert.ToBoolean(periods["weekly"]));
        Assert.False(Convert.ToBoolean(periods["monthly"]));    
        var responseBody = deserialized.Response;
    
        Assert.Equal(false, responseBody.ShouldEndSession);
    
        var outputSpeech = responseBody.OutputSpeech;
    
        Assert.Equal(typeof(PlainTextOutputSpeech), outputSpeech.GetType());
    
        var plainTextOutput = (PlainTextOutputSpeech)outputSpeech;
    
        Assert.Equal("PlainText", plainTextOutput.Type);
        Assert.Equal("Today will provide you a new learning opportunity. Stick with it and the possibilities will be endless. Can I help you with anything else?", plainTextOutput.Text);
    
        var card = responseBody.Card;
    
        Assert.Equal(typeof(SimpleCard), card.GetType());
    
        var simpleCard = (SimpleCard)card;
    
        Assert.Equal("Simple", simpleCard.Type);
        Assert.Equal("Horoscope", simpleCard.Title);
        Assert.Equal("Today will provide you a new learning opportunity. Stick with it and the possibilities will be endless.", simpleCard.Content);
    }

    [Fact]
    public void PlainTextConstructorSetsText()
    {
        var plainText = new PlainTextOutputSpeech("testing output");
        Assert.Equal("testing output", plainText.Text);
    }

    [Fact]
    public void SsmlTextConstructorSetsText()
    {
        var output = new Response.Ssml.Speech(new PlainText("testing output")).ToXml();
        var ssmlText = new SsmlOutputSpeech(output);
        Assert.Equal(output, ssmlText.Ssml);
    }

    [Fact]
    public void NewDirectiveSupport()
    {
        // Arrange
        var directive = new JsonDirective("UnknownDirectiveType");

        var nested = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(
            """{ "value": "test" }""");

        directive.Properties["testProperty"] = nested;
        var options = new JsonSerializerOptions
        {
            WriteIndented = false,
        };
        var jsonOutput = System.Text.Json.JsonSerializer.Serialize<IDirective>(directive, options);
        var outputDirective = System.Text.Json.JsonSerializer.Deserialize<IDirective>(jsonOutput, options);

        // Assert
        var jsonInput = Assert.IsType<JsonDirective>(outputDirective);
        Assert.Equal("UnknownDirectiveType", jsonInput.Type);
        Assert.True(jsonInput.Properties!.ContainsKey("testProperty"));

        var jsonElement = jsonInput.Properties["testProperty"];
        Assert.Equal(JsonValueKind.Object, jsonElement.ValueKind);
        Assert.True(jsonElement.TryGetProperty("value", out var valueProp));
        Assert.Equal("test", valueProp.GetString());
    }

    [Fact]
    public void EmptyDirectiveOrNoOverrideUsesSetValue()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        Assert.True(tell.Response.ShouldEndSession);

        tell.Response.Directives.Add(new JsonDirective("nothingspecial"));
        Assert.True(tell.Response.ShouldEndSession);
    }


    [Fact]
    public void HandlesExampleAskForPermissionsConsentDirective()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("AskForPermissionsConsentDirective.json");
        var askFor = Assert.IsType<AskForPermissionDirective>(deserialized);
        Assert.Equal(1.ToString(),askFor.Payload.Version);
        Assert.Equal("AskFor",askFor.Name);
        Assert.Equal("alexa::alerts:reminders:skill:readwrite",askFor.Payload.PermissionScope);
        Utility.CompareJson(askFor, "AskForPermissionsConsentDirective.json");
    }
}