using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl;

public class UserEventRequestHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == UserEventRequest.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(UserEventRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<UserEventRequestHandler>();

    }
}