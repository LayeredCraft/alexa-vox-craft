using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Ssml;

namespace AlexaVoxCraft.MediatR.Response;

public class DefaultResponseBuilder : IResponseBuilder
{
    private readonly IAttributesManager _attributesManager;
    private readonly ResponseBody _response;

    public DefaultResponseBuilder(IAttributesManager attributesManager)
    {
        _attributesManager = attributesManager ?? throw new ArgumentNullException(nameof(attributesManager));
        _response = new();
    }

    public IResponseBuilder Speak(string? speechOutput)
    {
        speechOutput = TrimOutputSpeech(speechOutput);
        return Speak(new PlainText(speechOutput));
    }

    public IResponseBuilder Speak(params ISsml[] elements)
    {
        _response.OutputSpeech = new SsmlOutputSpeech
        {
            Ssml = new Speech(elements).ToXml()
        };

        return this;
    }

    public IResponseBuilder SpeakAudio(string? audioUrl)
    {
        audioUrl = TrimOutputSpeech(audioUrl);
        return Speak(new Audio(audioUrl));
    }

    public IResponseBuilder Reprompt(string? repromptSpeechOutput)
    {
        repromptSpeechOutput = TrimOutputSpeech(repromptSpeechOutput);
        return Reprompt(new PlainText(repromptSpeechOutput));
    }

    public IResponseBuilder Reprompt(params ISsml[] elements)
    {
        _response.Reprompt = new Reprompt
        {
            OutputSpeech = new SsmlOutputSpeech
            {
                Ssml = new Speech(elements).ToXml()
            }
        };

        if (!IsVideoAppLaunchDirectivePresent())
        {
            _response.ShouldEndSession = false;
        }

        return this;
    }

    public IResponseBuilder WithSimpleCard(string cardTitle, string cardContent)
    {
        _response.Card = new SimpleCard
        {
            Title = cardTitle,
            Content = cardContent
        };

        return this;
    }

    public IResponseBuilder WithStandardCard(string cardTitle, string cardContent, string? smallImageUrl = null,
        string? largeImageUrl = null)
    {
        var card = new StandardCard
        {
            Title = cardTitle,
            Content = cardContent
        };

        if (!string.IsNullOrWhiteSpace(smallImageUrl) || !string.IsNullOrWhiteSpace(largeImageUrl))
        {
            card.Image = new CardImage
            {
                SmallImageUrl = smallImageUrl,
                LargeImageUrl = largeImageUrl
            };
        }

        _response.Card = card;

        return this;
    }

    public IResponseBuilder WithLinkAccountCard()
    {
        _response.Card = new LinkAccountCard();

        return this;
    }

    public IResponseBuilder WithAskForPermissionsConsentCard(List<string> permissionArray)
    {
        _response.Card = new AskForPermissionsConsentCard
        {
            Permissions = permissionArray
        };

        return this;
    }

    public IResponseBuilder AddAudioPlayerPlayDirective(PlayBehavior playBehavior, string url, string token,
        int offsetInMilliseconds, string? expectedPreviousToken = null, AudioItemMetadata? audioItemMetadata = null,
        CancellationToken cancellationToken = default)
    {
        var stream = new AudioItemStream
        {
            Url = url,
            Token = token,
            OffsetInMilliseconds = offsetInMilliseconds
        };

        if (!string.IsNullOrWhiteSpace(expectedPreviousToken))
            stream.ExpectedPreviousToken = expectedPreviousToken;

        var audioItem = new AudioItem
        {
            Stream = stream
        };

        if (audioItemMetadata is not null)
        {
            audioItem.Metadata = audioItemMetadata;
        }

        var playDirective = new AudioPlayerPlayDirective
        {
            PlayBehavior = playBehavior,
            AudioItem = audioItem
        };

        return AddDirective(playDirective);
    }

    public IResponseBuilder AddAudioPlayerStopDirective()
    {
        return AddDirective(new StopDirective());
    }

    public IResponseBuilder AddAudioPlayerClearQueueDirective(ClearBehavior clearBehavior)
    {
        return AddDirective(new ClearQueueDirective
        {
            ClearBehavior = clearBehavior
        });
    }

    public IResponseBuilder AddConfirmIntentDirective(Intent? updatedIntent = null)
    {
        var confirmIntentDirective = new DialogConfirmIntent();
        if (updatedIntent is not null)
        {
            confirmIntentDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(confirmIntentDirective);
    }

    public IResponseBuilder AddConfirmSlotDirective(string slotToConfirm, Intent? updatedIntent = null)
    {
        var confirmSlotDirective = new DialogConfirmSlot(slotToConfirm);
        if (updatedIntent is not null)
        {
            confirmSlotDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(confirmSlotDirective);
    }

    public IResponseBuilder AddDelegateDirective(Intent? updatedIntent = null)
    {
        var delegateDirective = new DialogDelegate();

        if (updatedIntent is not null)
        {
            delegateDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(delegateDirective);
    }

    public IResponseBuilder AddDirective(IDirective directive)
    {
        _response.Directives ??= new List<IDirective>();

        _response.Directives.Add(directive);

        return this;
    }

    public IResponseBuilder AddElicitSlotDirective(string slotToElicit, Intent? updatedIntent = null)
    {
        var elicitSlotDirective = new DialogElicitSlot(slotToElicit);

        if (updatedIntent is not null)
        {
            elicitSlotDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(elicitSlotDirective);
    }

    public IResponseBuilder AddHintDirective(string text)
    {
        return AddDirective(new HintDirective
        {
            Hint = new Hint
            {
                Type = "PlainText",
                Text = text
            }
        });
    }

    public IResponseBuilder AddVideoAppLaunchDirective(string source, string? title = null, string? subtitle = null)
    {
        var videoItem = new VideoItem(source);

        if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(subtitle))
        {
            videoItem.Metadata = new VideoItemMetadata
            {
                Subtitle = subtitle,
                Title = title
            };
        }

        _response.ShouldEndSession = null;

        return AddDirective(new VideoAppDirective
        {
            VideoItem = videoItem
        });
    }

    public IResponseBuilder WithShouldEndSession(bool val)
    {
        if (!IsVideoAppLaunchDirectivePresent())
            _response.ShouldEndSession = val;

        return this;
    }

    public async Task<SkillResponse> GetResponse(CancellationToken cancellationToken = default)
    {
        var response = new SkillResponse
        {
            Version = "1.0",
            SessionAttributes = (await _attributesManager.GetSessionAttributes(cancellationToken)).ToDictionary(),
            Response = _response
        };
        return response;
    }

    private bool IsVideoAppLaunchDirectivePresent()
    {
        return _response.Directives?.Any(d => d is VideoAppDirective) ?? false;
    }

    private string TrimOutputSpeech(string? outputSpeech)
    {
        if (string.IsNullOrWhiteSpace(outputSpeech))
            return string.Empty;

        var speechSpan = outputSpeech.AsSpan();
        speechSpan = speechSpan.Trim();
        var start = "<speak>".AsSpan();
        var end = "</speak>".AsSpan();
        if (speechSpan.StartsWith(start) && speechSpan.EndsWith(end))
        {
            return speechSpan.Slice(start.Length, speechSpan.Length - start.Length - end.Length).Trim().ToString();
        }

        return speechSpan.ToString();
    }
}