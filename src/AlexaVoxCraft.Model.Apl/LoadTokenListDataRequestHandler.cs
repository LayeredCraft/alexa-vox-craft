using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl;

public class LoadTokenListDataRequestHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == LoadTokenListDataRequest.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(LoadTokenListDataRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<LoadTokenListDataRequestHandler>();
    }
}