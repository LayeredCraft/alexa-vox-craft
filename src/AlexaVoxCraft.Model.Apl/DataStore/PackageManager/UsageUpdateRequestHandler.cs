using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl.DataStore.PackageManager;

public class UpdateRequestHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == UpdateRequest.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(UpdateRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<UpdateRequestHandler>();
    }
}