namespace AlexaVoxCraft.Model.Request.Type.Converters;

public class PlaybackRequestTypeResolver : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType.StartsWith("PlaybackController");
    }

    public System.Type? Resolve(string requestType)
    {
        return  typeof(PlaybackControllerRequest);
    }
}