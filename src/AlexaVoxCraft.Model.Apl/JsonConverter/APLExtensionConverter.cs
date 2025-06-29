using System;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Apl.Extensions.Backstack;
using AlexaVoxCraft.Model.Apl.Extensions.DataStore;
using AlexaVoxCraft.Model.Apl.Extensions.EntitySensing;
using AlexaVoxCraft.Model.Apl.Extensions.SmartMotion;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLExtensionConverter : BasePolymorphicConverter<APLExtension>
{
    private static readonly Dictionary<string, Type> AplExtensionLookup = new Dictionary<string, Type>
    {
        { BackstackExtension.URL, typeof(BackstackExtension) },
        { SmartMotionExtension.URL, typeof(SmartMotionExtension) },
        { EntitySensingExtension.URL, typeof(EntitySensingExtension) },
        { DataStoreExtension.URL, typeof(DataStoreExtension) }
    };

    protected override string TypeDiscriminatorPropertyName => "uri";
    protected override IDictionary<string, Type> DerivedTypes => AplExtensionLookup;
}