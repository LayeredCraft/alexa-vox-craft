using System;
using System.Collections.Generic;
using System.Text.Json;
using AlexaVoxCraft.Model.Apl.VectorGraphics;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class AVGItemConverter : BasePolymorphicConverter<IAVGItem>
{
    protected override string TypeDiscriminatorPropertyName => "type";

    protected override IDictionary<string, Type> DerivedTypes => new Dictionary<string, Type>
    {
        { "path", typeof(AVGPath) },
        { "group", typeof(AVGGroup) },
        { "text", typeof(AVGText) }
    };

    protected override IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories =>
        new Dictionary<string, Func<JsonElement, Type>>();

    protected override Func<JsonElement, Type?>? CustomTypeResolver => null;
    protected override Type? DefaultType => null;
}