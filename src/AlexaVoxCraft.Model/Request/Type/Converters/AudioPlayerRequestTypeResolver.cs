namespace AlexaVoxCraft.Model.Request.Type.Converters;

public class AudioPlayerRequestTypeResolver : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType.StartsWith("AudioPlayer");
    }

    public System.Type? Resolve(string requestType)
    {
        return typeof(AudioPlayerRequest);
    }
}