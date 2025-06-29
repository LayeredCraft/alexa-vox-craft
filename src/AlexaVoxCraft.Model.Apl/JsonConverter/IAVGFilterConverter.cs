using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class IAVGFilterConverter : BasePolymorphicConverter<IAVGFilter>
{
    private static readonly ConcurrentDictionary<string, Type> IavgFilterLookup = new()
    {
        [nameof(DropShadow)] = typeof(DropShadow),
    };
    protected override IDictionary<string, Type> DerivedTypes => IavgFilterLookup;
}