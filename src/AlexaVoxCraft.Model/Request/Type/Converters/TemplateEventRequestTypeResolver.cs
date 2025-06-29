namespace AlexaVoxCraft.Model.Request.Type.Converters;

public class TemplateEventRequestTypeResolver : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == "Display.ElementSelected";
    }

    public System.Type? Resolve(string requestType)
    {
        return typeof(DisplayElementSelectedRequest);
    }
}