using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Ssml;

namespace AlexaVoxCraft.MediatR.Response;

public interface IResponseBuilder
{
    #region Builder Methods
    
    #region Speech Methods
    
    IResponseBuilder Speak(string? speechOutput);

    IResponseBuilder Speak(params ISsml[] elements);

    IResponseBuilder SpeakAudio(string? audioUrl);

    IResponseBuilder Reprompt(string? repromptSpeechOutput);

    IResponseBuilder Reprompt(params ISsml[] elements);

    IResponseBuilder WithSimpleCard(string cardTitle, 
        string cardContent);

    IResponseBuilder WithStandardCard(string cardTitle, 
        string cardContent, 
        string? smallImageUrl = null, 
        string? largeImageUrl = null);

    IResponseBuilder WithLinkAccountCard();

    IResponseBuilder WithAskForPermissionsConsentCard(List<string> permissionArray);
    
    #endregion

    #region Directive Methods

    IResponseBuilder AddAudioPlayerPlayDirective(PlayBehavior playBehavior, string url, string token,
        int offsetInMilliseconds, string? expectedPreviousToken = null, AudioItemMetadata? audioItemMetadata = null,
        CancellationToken cancellationToken = default);
    
    IResponseBuilder AddAudioPlayerStopDirective();

    IResponseBuilder AddAudioPlayerClearQueueDirective(ClearBehavior clearBehavior);

    IResponseBuilder AddConfirmIntentDirective(Intent? updatedIntent = null);

    IResponseBuilder AddConfirmSlotDirective(string slotToConfirm, Intent? updatedIntent = null);

    IResponseBuilder AddDelegateDirective(Intent? updatedIntent = null);

    IResponseBuilder AddDirective(IDirective directive);

    IResponseBuilder AddElicitSlotDirective(string slotToElicit, Intent? updatedIntent = null);

    IResponseBuilder AddHintDirective(string text);

    IResponseBuilder AddVideoAppLaunchDirective(string source, string? title = null, string? subtitle = null);

    #endregion

    IResponseBuilder WithShouldEndSession(bool val);

    Task<SkillResponse> GetResponse(CancellationToken cancellationToken = default);

    #endregion
}