using System;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Apl.DataStore;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class DataStoreErrorConverter: BasePolymorphicConverter<DataStoreError>
{
    private static readonly Dictionary<string, Type> DataStoreErrorLookup = new()
    {
        { "STORAGE_LIMIT_EXCEEDED", typeof(DataStoreStorageError) },
        { "DATASTORE_INTERNAL_ERROR", typeof(DataStoreStorageError) },
        { "DEVICE_UNAVAILABLE", typeof(DataStoreDeviceError) },
        { "DEVICE_PERMANENTLY_UNAVAILABLE", typeof(DataStoreDeviceError) },
        { "INVALID_TARGET", typeof(DataStoreDeviceError)}
    };

    protected override IDictionary<string, Type> DerivedTypes => DataStoreErrorLookup;
}