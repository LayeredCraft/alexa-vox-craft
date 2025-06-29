using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.ConnectionTasks;

public static class IConnectionTaskExtensions
{

    public static StartConnectionDirective ToConnectionDirective(this IConnectionTask task, string token = null)
    {
        return new StartConnectionDirective(task, token);
    }
}