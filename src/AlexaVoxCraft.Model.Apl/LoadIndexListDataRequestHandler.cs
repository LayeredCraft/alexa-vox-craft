using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl;

public class LoadIndexListDataRequestHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == LoadIndexListDataRequest.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(LoadIndexListDataRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<LoadIndexListDataRequestHandler>();
    }
}