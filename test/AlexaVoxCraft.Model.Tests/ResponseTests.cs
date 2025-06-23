using System.Text.Json;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Directive.Templates;
using AlexaVoxCraft.Model.Response.Ssml;
using Xunit;

namespace AlexaVoxCraft.Model.Tests;

public class ResponseTests
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
    public void Creates_VideoAppDirective()
    {
        var videoItem = new VideoItem("https://www.example.com/video/sample-video-1.mp4")
        {
            Metadata = new VideoItemMetadata
            {
                Title = "Title for Sample Video",
                Subtitle = "Secondary Title for Sample Video"
            }
        };
        var actual = new VideoAppDirective { VideoItem = videoItem };

        Assert.True(Utility.CompareJson(actual, "VideoAppDirectiveWithMetadata.json"));
    }

    [Fact]
    public void Create_VideoAppDirective_FromSource()
    {
        var actual = new VideoAppDirective("https://www.example.com/video/sample-video-1.mp4");
        Assert.True(Utility.CompareJson(actual, "VideoAppDirective.json"));
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
    public void AudioPlayerGeneratesCorrectJson()
    {
        var directive = new AudioPlayerPlayDirective
        {
            PlayBehavior = PlayBehavior.Enqueue,
            AudioItem = new AudioItem
            {
                Stream = new AudioItemStream
                {
                    Url = "https://url-of-the-stream-to-play",
                    Token = "opaque token representing this stream",
                    ExpectedPreviousToken = "opaque token representing the previous stream"
                }
            }
        };
        Assert.True(Utility.CompareJson(directive, "AudioPlayerWithoutMetadata.json"));
    }

    [Fact]
    public void AudioPlayerWithMetadataGeneratesCorrectJson()
    {
        var directive = new AudioPlayerPlayDirective
        {
            PlayBehavior = PlayBehavior.Enqueue,
            AudioItem = new AudioItem
            {
                Stream = new AudioItemStream
                {
                    Url = "https://url-of-the-stream-to-play",
                    Token = "opaque token representing this stream",
                    ExpectedPreviousToken = "opaque token representing the previous stream"
                },
                Metadata = new AudioItemMetadata
                {
                    Title = "title of the track to display",
                    Subtitle = "subtitle of the track to display",
                    Art = new AudioItemSources
                    {
                        Sources = new[] { new AudioItemSource("https://url-of-the-album-art-image.png") }.ToList()
                    },
                    BackgroundImage = new AudioItemSources { Sources = new[] { new AudioItemSource("https://url-of-the-background-image.png") }.ToList() }
                }
            }
        };
        Assert.True(Utility.CompareJson(directive, "AudioPlayerWithMetadata.json"));
    }

    [Fact]
    public void AudioPlayerWithMetadataDeserializesCorrectly()
    {
        var audioPlayer = Utility.ExampleFileContent<AudioPlayerPlayDirective>("AudioPlayerWithMetadata.json");
        Assert.Equal("title of the track to display", audioPlayer.AudioItem.Metadata.Title);
        Assert.Equal("subtitle of the track to display", audioPlayer.AudioItem.Metadata.Subtitle);
        Assert.Single(audioPlayer.AudioItem.Metadata.Art.Sources);
        Assert.Single(audioPlayer.AudioItem.Metadata.BackgroundImage.Sources);
        Assert.Equal("https://url-of-the-album-art-image.png", audioPlayer.AudioItem.Metadata.Art.Sources.First().Url);
        Assert.Equal("https://url-of-the-background-image.png", audioPlayer.AudioItem.Metadata.BackgroundImage.Sources.First().Url);
    }

    [Fact]
    public void AudioPlayerIgnoresMetadataWhenNull()
    {
        var audioPlayer = Utility.ExampleFileContent<AudioPlayerPlayDirective>("AudioPlayerWithoutMetadata.json");
        Assert.Null(audioPlayer.AudioItem.Metadata);
        Assert.Equal("https://url-of-the-stream-to-play", audioPlayer.AudioItem.Stream.Url);
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
        var speech = new Speech(new PlainText("text"));
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
    public void DeserializesExampleAudioPlayerWithMetadataJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("AudioPlayerWithMetadata.json");

        Assert.Equal(typeof(AudioPlayerPlayDirective), deserialized.GetType());

        var directive = (AudioPlayerPlayDirective)deserialized;

        Assert.Equal("AudioPlayer.Play", directive.Type);
        Assert.Equal(PlayBehavior.Enqueue, directive.PlayBehavior);

        var stream = directive.AudioItem.Stream;

        Assert.Equal("https://url-of-the-stream-to-play", stream.Url);
        Assert.Equal("opaque token representing this stream", stream.Token);
        Assert.Equal("opaque token representing the previous stream", stream.ExpectedPreviousToken);
        Assert.Equal(0, stream.OffsetInMilliseconds);

        var metadata = directive.AudioItem.Metadata;

        Assert.Equal("title of the track to display", metadata.Title);
        Assert.Equal("subtitle of the track to display", metadata.Subtitle);
        Assert.Single(metadata.Art.Sources);
        Assert.Equal("https://url-of-the-album-art-image.png", metadata.Art.Sources.First().Url);
        Assert.Single(metadata.BackgroundImage.Sources);
        Assert.Equal("https://url-of-the-background-image.png", metadata.BackgroundImage.Sources.First().Url);
    }

    [Fact]
    public void DeserializesExampleClearQueueDirectiveJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("ClearQueueDirective.json");

        Assert.Equal(typeof(ClearQueueDirective), deserialized.GetType());

        var directive = (ClearQueueDirective)deserialized;

        Assert.Equal("AudioPlayer.ClearQueue", directive.Type);
        Assert.Equal(ClearBehavior.ClearAll, directive.ClearBehavior);
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
    public void DeserializesExampleStopDirectiveJson()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("StopDirective.json");

        Assert.Equal(typeof(StopDirective), deserialized.GetType());

        var directive = (StopDirective)deserialized;

        Assert.Equal("AudioPlayer.Stop", directive.Type);
    }

    [Fact]
    public void DeserializesExampleDirectiveVideoAppDirectiveWithMetadata()
    {
        var deserialized = Utility.ExampleFileContent<IDirective>("VideoAppDirectiveWithMetadata.json");

        Assert.Equal(typeof(VideoAppDirective), deserialized.GetType());

        var directive = (VideoAppDirective)deserialized;

        Assert.Equal("VideoApp.Launch", directive.Type);
        Assert.Equal("https://www.example.com/video/sample-video-1.mp4", directive.VideoItem.Source);
        Assert.Equal("Title for Sample Video", directive.VideoItem.Metadata.Title);
        Assert.Equal("Secondary Title for Sample Video", directive.VideoItem.Metadata.Subtitle);
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
        var output = new Speech(new PlainText("testing output")).ToXml();
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
    public void DirectiveShouldEndSessionOverrideSupport()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        Assert.True(tell.Response.ShouldEndSession);

        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        Assert.Null(tell.Response.ShouldEndSession);
    }

    [Fact]
    public void MultipleShouldEndDirectivesWithCommonRequirement()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        Assert.True(tell.Response.ShouldEndSession);

        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        Assert.Null(tell.Response.ShouldEndSession);
    }

    [Fact]
    public void ContradictingEndSessionOverrideDefaultsToExplicit()
    {
        var tell = ResponseBuilder.Tell("this should end the session");
        Assert.True(tell.Response.ShouldEndSession);

        //As VideoApp needs a null EndSession and FakeDirective needs false - reverts to explicit
        tell.Response.Directives.Add(new VideoAppDirective("https://example.com/test.mp4"));
        tell.Response.Directives.Add(new FakeDirective());
        Assert.True(tell.Response.ShouldEndSession);
    }

    private class FakeDirective : IEndSessionDirective
    {
        public string Type => "fake";
        public bool? ShouldEndSession => false;
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