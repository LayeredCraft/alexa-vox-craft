using System;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Apl.Gestures;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLGestureConverter : BasePolymorphicConverter<APLGesture>
{
    private static readonly Dictionary<string, Type> AplGestureLookup = new()
    {
        {nameof(DoublePress), typeof(DoublePress)},
        {nameof(LongPress), typeof(LongPress)},
        {nameof(SwipeAway), typeof(SwipeAway)},
        {nameof(Tap), typeof(Tap)}
    };

    protected override IDictionary<string, Type> DerivedTypes => AplGestureLookup;
}