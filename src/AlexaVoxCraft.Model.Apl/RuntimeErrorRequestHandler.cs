using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl;

public class RuntimeErrorRequestHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == RuntimeErrorRequest.RequestType;
    }

    public Type Resolve(string requestType)
    {
        return typeof(RuntimeErrorRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<RuntimeErrorRequestHandler>();
    }
}