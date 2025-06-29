using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class InstallationErrorHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == InstallationError.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(InstallationError);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<InstallationErrorHandler>();
    }

}