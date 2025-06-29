namespace AlexaVoxCraft.Model.Request.Type.Converters;

public class SkillConnectionRequestTypeResolver:IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == "SessionResumedRequest";
    }

    public System.Type? Resolve(string requestType)
    {
        return typeof(SessionResumedRequest);
    }
}