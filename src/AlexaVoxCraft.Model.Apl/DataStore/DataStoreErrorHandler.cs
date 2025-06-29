using System;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Apl.DataStore;

public class DataStoreErrorHandler : IRequestTypeResolver
{
    public bool CanResolve(string requestType)
    {
        return requestType == DataStoreErrorRequest.RequestType || requestType == "Alexa.DataStore.Error";
    }

    public Type Resolve(string requestType)
    {
        return typeof(DataStoreErrorRequest);
    }

    public static void AddToRequestConverter()
    {
        RequestConverter.RegisterRequestTypeResolver<DataStoreErrorHandler>();
    }
}