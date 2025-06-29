using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Apl.Filters;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class ImageFilterConverter : BasePolymorphicConverter<IImageFilter>
{

    public static ConcurrentDictionary<string, Type> ImageFilterLookup = new()
    {
        [nameof(Blend)] = typeof(Blend),
        [nameof(Blur)] = typeof(Blur),
        [nameof(Color)] = typeof(Color),
        [nameof(Gradient)] = typeof(Gradient),
        [nameof(Grayscale)] = typeof(Grayscale),
        [nameof(Noise)] = typeof(Noise),
        [nameof(Saturate)] = typeof(Saturate),
    };

    protected override IDictionary<string, Type> DerivedTypes => ImageFilterLookup;
}