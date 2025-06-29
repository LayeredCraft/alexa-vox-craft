using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Request.Type;

public class AudioPlayerRequest: Request
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("offsetInMilliseconds")]
    public long OffsetInMilliseconds { get; set; }

    [JsonPropertyName("error")]
    public Error Error { get; set; }

    [JsonPropertyName("currentPlaybackState")]
    public PlaybackState CurrentPlaybackState { get; set; }

    [JsonPropertyName("enqueuedToken")]
    public string EnqueuedToken { get; set; }
        
    public bool HasEnqueuedItem
    {
        get
        {
            /*if (EnqueuedToken != null && EnqueuedToken != "")
                return true;
            else
                return false;
                */
            return !String.IsNullOrEmpty(EnqueuedToken);
        }
    }

    public AudioRequestType AudioRequestType
    {
        get
        {
            switch (this.Type.Split('.')[1])
            {
                case "PlaybackStarted":
                    return AudioRequestType.PlaybackStarted;
                case "PlaybackFinished":
                    return AudioRequestType.PlaybackFinished;
                case "PlaybackStopped":
                    return AudioRequestType.PlaybackStopped;
                case "PlaybackNearlyFinished":
                    return AudioRequestType.PlaybackNearlyFinished;
                case "PlaybackFailed":
                    return AudioRequestType.PlaybackFailed;
                default:
                    return AudioRequestType.Unknown;
            }
        }
    }
}