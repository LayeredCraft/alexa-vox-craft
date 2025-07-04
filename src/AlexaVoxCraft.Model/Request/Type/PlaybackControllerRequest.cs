﻿namespace AlexaVoxCraft.Model.Request.Type;

public class PlaybackControllerRequest : Request
{
    public PlaybackControllerRequestType PlaybackRequestType
    {
        get
        {
            switch (this.Type.Split('.')[1])
            {
                case "NextCommandIssued":
                    return PlaybackControllerRequestType.Next;
                case "PauseCommandIssued":
                    return PlaybackControllerRequestType.Pause;
                case "PlayCommandIssued":
                    return PlaybackControllerRequestType.Play;
                case "PreviousCommandIssued":
                    return PlaybackControllerRequestType.Previous;
                default:
                    return PlaybackControllerRequestType.Unknown;
            }
        }
    }
}