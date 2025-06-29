namespace AlexaVoxCraft.Model.Request.Type.Converters;

public class SkillEventRequestTypeResolver : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == "AlexaSkillEvent.SkillPermissionAccepted" ||
               requestType == "AlexaSkillEvent.SkillPermissionChanged" ||
               requestType == "AlexaSkillEvent.SkillAccountLinked" ||
               requestType.StartsWith("AlexaSkillEvent");
    }

    public System.Type Resolve(string requestType)
    {
        return requestType switch
        {
            "AlexaSkillEvent.SkillAccountLinked" => typeof(AccountLinkSkillEventRequest),
            "AlexaSkillEvent.SkillPermissionAccepted" or "AlexaSkillEvent.SkillPermissionChanged" =>
                typeof(PermissionSkillEventRequest),
            "AlexaSkillEvent.SkillDisabled" or "AlexaSkillEvent.SkillEnabled" =>
                typeof(SkillEnablementSkillEventRequest),
            _ => typeof(SkillEventRequest)
        };
    }
}