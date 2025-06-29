using System;
using System.Collections.Generic;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class ViewportConverter : BasePolymorphicConverter<Viewport>
{
    protected override IDictionary<string, Type> DerivedTypes => new Dictionary<string, Type>
    {
        [APLViewport.ViewportType] = typeof(APLViewport),
        [APLTViewport.ViewportType] = typeof(APLTViewport)
    };
}