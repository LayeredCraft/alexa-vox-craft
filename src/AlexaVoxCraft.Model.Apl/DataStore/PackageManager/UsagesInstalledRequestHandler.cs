using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class UsagesInstalledRequestHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == UsagesInstalledRequest.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(UsagesInstalledRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<UsagesInstalledRequestHandler>();
    }
}