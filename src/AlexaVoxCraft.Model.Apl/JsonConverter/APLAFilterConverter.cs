using System;
using System.Collections.Generic;
using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Audio.Filters;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLAFilterConverter: BasePolymorphicConverter<APLAFilter>
{
    private static Dictionary<string, Type> _derivedTypes = new()
    {
        {nameof(Trim), typeof(Trim)},
        {nameof(FadeIn), typeof(FadeIn)},
        {nameof(FadeOut), typeof(FadeOut)},
        {nameof(Volume), typeof(Volume)},
        {nameof(Repeat), typeof(Repeat) }
    };


    protected override string TypeDiscriminatorPropertyName => "type";
    protected override IDictionary<string, Type> DerivedTypes => _derivedTypes;

    protected override IDictionary<string, Func<JsonElement, Type>> DataDrivenTypeFactories =>
        new Dictionary<string, Func<JsonElement, Type>>();

    protected override Func<JsonElement, Type?>? CustomTypeResolver => null;
    protected override Type? DefaultType => null;
}