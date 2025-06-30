using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class UsagesRemovedRequestHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == UsagesRemovedRequest.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(UsagesRemovedRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<UsagesRemovedRequestHandler>();
    }
}