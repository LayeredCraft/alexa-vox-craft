using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.Filters;
using AlexaVoxCraft.Model.Apl.VectorGraphics;
using AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLValueCollectionConverterFactory : JsonConverterFactory
{
    // Closed dispatch for every APLValueCollection<T> instantiation this library ships, keyed by T (not
    // APLValueCollection<T>) - compile-time-closed, no MakeGenericType/Activator.CreateInstance. Assembled
    // from every APLValueCollection<...> declaration under src/, test/, samples/ during Task Group 4
    // planning; re-verify this list stays complete whenever a new APLValueCollection<T>-typed property is
    // added. All always default to `alwaysOutputArray: true` (backward compatible with the removed
    // APLEnumerableValueConverter).
    private static readonly Dictionary<Type, Func<System.Text.Json.Serialization.JsonConverter>> _itemConverters = new()
    {
        { typeof(APLAComponent), static () => new APLValueCollectionConverter<APLAComponent>(true) },
        { typeof(APLAFilter), static () => new APLValueCollectionConverter<APLAFilter>(true) },
        { typeof(APLAction), static () => new APLValueCollectionConverter<APLAction>(true) },
        { typeof(APLCommand), static () => new APLValueCollectionConverter<APLCommand>(true) },
        { typeof(APLComponent), static () => new APLValueCollectionConverter<APLComponent>(true) },
        { typeof(APLDimensionValue), static () => new APLValueCollectionConverter<APLDimensionValue>(true) },
        { typeof(APLExtension), static () => new APLValueCollectionConverter<APLExtension>(true) },
        { typeof(APLGesture), static () => new APLValueCollectionConverter<APLGesture>(true) },
        { typeof(APLKeyboardHandler), static () => new APLValueCollectionConverter<APLKeyboardHandler>(true) },
        { typeof(APLPageMoveHandler), static () => new APLValueCollectionConverter<APLPageMoveHandler>(true) },
        { typeof(APLTransform), static () => new APLValueCollectionConverter<APLTransform>(true) },
        { typeof(APLValue<int?>), static () => new APLValueCollectionConverter<APLValue<int?>>(true) },
        { typeof(AVGParameter), static () => new APLValueCollectionConverter<AVGParameter>(true) },
        { typeof(AlexaImageListItem), static () => new APLValueCollectionConverter<AlexaImageListItem>(true) },
        { typeof(AlexaListItem), static () => new APLValueCollectionConverter<AlexaListItem>(true) },
        { typeof(AlexaPaginatedListItem), static () => new APLValueCollectionConverter<AlexaPaginatedListItem>(true) },
        { typeof(AlexaTextListItem), static () => new APLValueCollectionConverter<AlexaTextListItem>(true) },
        { typeof(AnimatedProperty), static () => new APLValueCollectionConverter<AnimatedProperty>(true) },
        { typeof(IAVGFilter), static () => new APLValueCollectionConverter<IAVGFilter>(true) },
        { typeof(IAVGItem), static () => new APLValueCollectionConverter<IAVGItem>(true) },
        { typeof(IImageFilter), static () => new APLValueCollectionConverter<IImageFilter>(true) },
        { typeof(IngredientListItem), static () => new APLValueCollectionConverter<IngredientListItem>(true) },
        { typeof(Parameter), static () => new APLValueCollectionConverter<Parameter>(true) },
        { typeof(TextTrack), static () => new APLValueCollectionConverter<TextTrack>(true) },
        { typeof(TickHandler), static () => new APLValueCollectionConverter<TickHandler>(true) },
        { typeof(VideoSource), static () => new APLValueCollectionConverter<VideoSource>(true) },
        { typeof(VisibilityChangeHandler), static () => new APLValueCollectionConverter<VisibilityChangeHandler>(true) },
        { typeof(double), static () => new APLValueCollectionConverter<double>(true) },
        { typeof(int), static () => new APLValueCollectionConverter<int>(true) },
        { typeof(object), static () => new APLValueCollectionConverter<object>(true) },
        { typeof(string), static () => new APLValueCollectionConverter<string>(true) },
    };

    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(APLValueCollection<>);
    }

    public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];

        if (_itemConverters.TryGetValue(itemType, out var factory))
        {
            return factory();
        }

        throw new NotSupportedException(
            $"APLValueCollection<{itemType}> has no registered converter. Every APLValueCollection<T> " +
            "instantiation this library ships must be registered in APLValueCollectionConverterFactory's " +
            "closed dispatch table.");
    }
}
