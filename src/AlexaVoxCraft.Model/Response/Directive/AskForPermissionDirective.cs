namespace AlexaVoxCraft.Model.Response.Directive;

public class AskForPermissionDirective : ConnectionSendRequest<AskForPermissionPayload>
{
    private const string RequestName = "AskFor";

    public AskForPermissionDirective()
    {
        Name = RequestName;
    }

    public AskForPermissionDirective(string permissionScope)
    {
        Name = RequestName;
        Payload = new AskForPermissionPayload(permissionScope);
    }
}