using System;
using System.Collections.Generic;
using System.Reflection;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class AnimatedPropertyConverter : BasePolymorphicConverter<AnimatedProperty>
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(AnimatedProperty).GetTypeInfo().IsAssignableFrom(typeof(AnimatedProperty));
    }
    
    protected override string TypeDiscriminatorPropertyName => "property";

    protected override IDictionary<string, Type> DerivedTypes => new Dictionary<string, Type>
    {
        { "opacity", typeof(AnimatedOpacity) },
        { "transform", typeof(AnimatedTransform) }
    };
}