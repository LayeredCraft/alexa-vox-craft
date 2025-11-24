using AlexaVoxCraft.MediatR.Attributes;
using AlexaVoxCraft.MediatR.DI;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Ssml;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.MediatR.Response;

/// <summary>
/// Default implementation of <see cref="IResponseBuilder"/> that provides a fluent API
/// for building Alexa skill responses with support for speech, cards, directives, and session management.
/// </summary>
/// <remarks>
/// <para>
/// This builder supports an optional default voice that wraps all speech output in an SSML
/// <c>&lt;voice&gt;</c> element. When configured via <see cref="SkillServiceConfiguration.DefaultVoiceName"/>,
/// all <see cref="Speak(string?)"/> and <see cref="Reprompt(string?)"/> calls will automatically
/// use the specified Amazon Polly voice.
/// </para>
/// <para>
/// Use <see cref="AlexaSupportedVoices"/> for available voice name constants that are supported by Alexa Skills.
/// </para>
/// </remarks>
public class DefaultResponseBuilder : IResponseBuilder
{
    private readonly IAttributesManager _attributesManager;
    private readonly ResponseBody _response;
    private readonly string? _defaultVoiceName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultResponseBuilder"/> class.
    /// </summary>
    /// <param name="attributesManager">The attributes manager for session state management.</param>
    /// <param name="skillServiceConfiguration">The skill service configuration containing optional default voice settings.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="attributesManager"/> is null.</exception>
    public DefaultResponseBuilder(IAttributesManager attributesManager,
        IOptions<SkillServiceConfiguration> skillServiceConfiguration)
    {
        _attributesManager = attributesManager ?? throw new ArgumentNullException(nameof(attributesManager));
        _response = new();
        _defaultVoiceName = skillServiceConfiguration.Value.DefaultVoiceName;
    }

    /// <inheritdoc />
    public IResponseBuilder Speak(string? speechOutput)
    {
        speechOutput = TrimOutputSpeech(speechOutput);
        return Speak(new PlainText(speechOutput));
    }

    /// <inheritdoc />
    /// <remarks>
    /// When a default voice is configured via <see cref="SkillServiceConfiguration.DefaultVoiceName"/>,
    /// the SSML elements will be wrapped in a <c>&lt;voice&gt;</c> element using that voice.
    /// </remarks>
    public IResponseBuilder Speak(params ISsml[] elements)
    {
        var effectiveElements = elements;
        if (!string.IsNullOrWhiteSpace(_defaultVoiceName))
        {
            var voiced = elements.WithVoice(_defaultVoiceName);
            effectiveElements = [voiced];
        }

        _response.OutputSpeech = new SsmlOutputSpeech
        {
            Ssml = new Speech(effectiveElements).ToXml()
        };

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder SpeakAudio(string? audioUrl)
    {
        audioUrl = TrimOutputSpeech(audioUrl);
        return Speak(new Audio(audioUrl));
    }

    /// <inheritdoc />
    public IResponseBuilder Reprompt(string? repromptSpeechOutput)
    {
        repromptSpeechOutput = TrimOutputSpeech(repromptSpeechOutput);
        return Reprompt(new PlainText(repromptSpeechOutput));
    }

    /// <inheritdoc />
    /// <remarks>
    /// When a default voice is configured via <see cref="SkillServiceConfiguration.DefaultVoiceName"/>,
    /// the SSML elements will be wrapped in a <c>&lt;voice&gt;</c> element using that voice.
    /// </remarks>
    public IResponseBuilder Reprompt(params ISsml[] elements)
    {
        var effectiveElements = elements;
        if (!string.IsNullOrWhiteSpace(_defaultVoiceName))
        {
            var voiced = elements.WithVoice(_defaultVoiceName);
            effectiveElements = [voiced];
        }

        _response.Reprompt = new Reprompt
        {
            OutputSpeech = new SsmlOutputSpeech
            {
                Ssml = new Speech(effectiveElements).ToXml()
            }
        };

        if (!IsVideoAppLaunchDirectivePresent())
        {
            _response.ShouldEndSession = false;
        }

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithSimpleCard(string cardTitle, string cardContent)
    {
        _response.Card = new SimpleCard
        {
            Title = cardTitle,
            Content = cardContent
        };

        return this;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IResponseBuilder WithLinkAccountCard()
    {
        _response.Card = new LinkAccountCard();

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithAskForPermissionsConsentCard(List<string> permissionArray)
    {
        _response.Card = new AskForPermissionsConsentCard
        {
            Permissions = permissionArray
        };

        return this;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IResponseBuilder AddAudioPlayerStopDirective()
    {
        return AddDirective(new StopDirective());
    }

    /// <inheritdoc />
    public IResponseBuilder AddAudioPlayerClearQueueDirective(ClearBehavior clearBehavior)
    {
        return AddDirective(new ClearQueueDirective
        {
            ClearBehavior = clearBehavior
        });
    }

    /// <inheritdoc />
    public IResponseBuilder AddConfirmIntentDirective(Intent? updatedIntent = null)
    {
        var confirmIntentDirective = new DialogConfirmIntent();
        if (updatedIntent is not null)
        {
            confirmIntentDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(confirmIntentDirective);
    }

    /// <inheritdoc />
    public IResponseBuilder AddConfirmSlotDirective(string slotToConfirm, Intent? updatedIntent = null)
    {
        var confirmSlotDirective = new DialogConfirmSlot(slotToConfirm);
        if (updatedIntent is not null)
        {
            confirmSlotDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(confirmSlotDirective);
    }

    /// <inheritdoc />
    public IResponseBuilder AddDelegateDirective(Intent? updatedIntent = null)
    {
        var delegateDirective = new DialogDelegate();

        if (updatedIntent is not null)
        {
            delegateDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(delegateDirective);
    }

    /// <inheritdoc />
    public IResponseBuilder AddDirective(IDirective directive)
    {
        _response.Directives ??= new List<IDirective>();

        _response.Directives.Add(directive);

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder AddElicitSlotDirective(string slotToElicit, Intent? updatedIntent = null)
    {
        var elicitSlotDirective = new DialogElicitSlot(slotToElicit);

        if (updatedIntent is not null)
        {
            elicitSlotDirective.UpdatedIntent = updatedIntent;
        }

        return AddDirective(elicitSlotDirective);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IResponseBuilder WithShouldEndSession(bool val)
    {
        if (!IsVideoAppLaunchDirectivePresent())
            _response.ShouldEndSession = val;

        return this;
    }

    /// <inheritdoc />
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