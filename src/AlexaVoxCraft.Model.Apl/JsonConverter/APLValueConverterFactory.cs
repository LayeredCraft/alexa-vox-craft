using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Audio;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.Filters;
using AlexaVoxCraft.Model.Apl.Gestures;
using AlexaVoxCraft.Model.Apl.VectorGraphics;
using AlexaVoxCraft.Model.Helpers;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLValueConverter<T> : JsonConverter<APLValue<T>>
{
    private static IList<JsonValueKind> _simpleTypes =
        [JsonValueKind.String, JsonValueKind.Number, JsonValueKind.False, JsonValueKind.True];

    public override APLValue<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null && (reader.TokenType != JsonTokenType.StartArray &&
                                                       reader.TokenType != JsonTokenType.StartObject))
        {
            return null;
        }

        var returnValue = new APLValue<T>();

        var genericType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        genericType = Nullable.GetUnderlyingType(genericType) ?? genericType;

        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (_simpleTypes.Contains(root.ValueKind) && !TypeMatches(root.ValueKind, genericType))
        {
            returnValue.Expression = root.ValueKind == JsonValueKind.String
                ? root.GetString()
                : root.GetRawText();
            return returnValue;
        }
        // Enum with [EnumMember] support
        if (genericType.IsEnum && root.ValueKind == JsonValueKind.String)
        {
            var str = root.GetString();
            if (EnumHelper.TryParseEnumWithEnumMemberSupport(genericType, str, out var parsedEnum))
            {
                returnValue.Value = (T)parsedEnum!;
            }
            else
            {
                returnValue.Expression = str;
            }

            return returnValue;
        }

        if (genericType == typeof(object))
        {
            returnValue.Value = (T)new ObjectConverter().Read(ref reader, typeToConvert, options)!;
        }
        else
        {
            returnValue.Value = (T)document.Deserialize(genericType, options)!;
        }

        return returnValue;
    }

    public override void Write(Utf8JsonWriter writer, APLValue<T> value, JsonSerializerOptions options)
    {
        var obj = !string.IsNullOrWhiteSpace(value.Expression) ? value.Expression : value.GetValue();
        JsonSerializer.Serialize(writer, obj, options);
    }

    private bool TypeMatches(JsonValueKind jsonValueKind, Type t)
    {
        return jsonValueKind switch
        {
            JsonValueKind.String when t.IsStringType() => true,
            JsonValueKind.False or JsonValueKind.True when typeof(bool) == t => true,
            JsonValueKind.Number when t.IsNumberType() => true,
            _ => false
        };
    }
}

internal static partial class TypeExtensions
{
    private static readonly IList<Type> NumberTypes =
        [typeof(int), typeof(short), typeof(long), typeof(double), typeof(float), typeof(decimal)];

    internal static bool IsStringType(this Type type) => type == typeof(string) || type.IsEnum;

    internal static bool IsNumberType(this Type type) => NumberTypes.Contains(type);
}

public class APLDimensionValueConverter : JsonConverter<APLDimensionValue>
{
    public override APLDimensionValue? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Number
            ? new APLDimensionValue(reader.GetInt32().ToString())
            : new APLDimensionValue(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, APLDimensionValue value, JsonSerializerOptions options)
    {
        var obj = !string.IsNullOrWhiteSpace(value.Expression) ? value.Expression : value.GetValue();
        JsonSerializer.Serialize(writer, obj, options);
    }
}

public class APLAbsoluteDimensionValueConverter : JsonConverter<APLAbsoluteDimensionValue>
{
    public override APLAbsoluteDimensionValue? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        return reader.TokenType == JsonTokenType.Number
            ? new APLAbsoluteDimensionValue(reader.GetInt32().ToString())
            : new APLAbsoluteDimensionValue(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, APLAbsoluteDimensionValue value, JsonSerializerOptions options)
    {
        var obj = !string.IsNullOrWhiteSpace(value.Expression) ? value.Expression : value.GetValue();
        JsonSerializer.Serialize(writer, obj, options);
    }
}

public class APLValueConverterFactory : JsonConverterFactory
{
    private static Type _aplDimensionType = typeof(APLDimensionValue);
    private static Type _aplAbsoluteDimensionType = typeof(APLAbsoluteDimensionValue);
    private static Type _aplObjectType = typeof(APLValue<object>);
    private static List<Type> _dimensionTypes = [_aplDimensionType, _aplAbsoluteDimensionType, _aplObjectType];
    private static readonly ConcurrentDictionary<Type, System.Text.Json.Serialization.JsonConverter?> _converterCache = new();

    // Closed dispatch for every APLValue<T> instantiation this library ships, keyed by T (not
    // APLValue<T>) - compile-time-closed, no MakeGenericType/Activator.CreateInstance. Assembled from
    // every APLValue<...> declaration under src/, test/, samples/ during Task Group 4 planning; re-verify
    // this list stays complete whenever a new APLValue<T>-typed property is added.
    private static readonly Dictionary<Type, Func<System.Text.Json.Serialization.JsonConverter>> _valueConverters = new()
    {
        { typeof(APLComponent), static () => new APLValueConverter<APLComponent>() },
        { typeof(APLDisplay), static () => new APLValueConverter<APLDisplay>() },
        { typeof(APLDisplay?), static () => new APLValueConverter<APLDisplay?>() },
        { typeof(APLGradient), static () => new APLValueConverter<APLGradient>() },
        { typeof(AVGParameterType), static () => new APLValueConverter<AVGParameterType>() },
        { typeof(AVGParameterType?), static () => new APLValueConverter<AVGParameterType?>() },
        { typeof(AVGScaleType), static () => new APLValueConverter<AVGScaleType>() },
        { typeof(AVGScaleType?), static () => new APLValueConverter<AVGScaleType?>() },
        { typeof(AlexaImageAlignment), static () => new APLValueConverter<AlexaImageAlignment>() },
        { typeof(AlexaImageAlignment?), static () => new APLValueConverter<AlexaImageAlignment?>() },
        { typeof(AlexaImageAspectRatio), static () => new APLValueConverter<AlexaImageAspectRatio>() },
        { typeof(AlexaImageAspectRatio?), static () => new APLValueConverter<AlexaImageAspectRatio?>() },
        { typeof(BlendMode), static () => new APLValueConverter<BlendMode>() },
        { typeof(BlendMode?), static () => new APLValueConverter<BlendMode?>() },
        { typeof(ContainerWrap), static () => new APLValueConverter<ContainerWrap>() },
        { typeof(ContainerWrap?), static () => new APLValueConverter<ContainerWrap?>() },
        { typeof(ContentDirection), static () => new APLValueConverter<ContentDirection>() },
        { typeof(ContentDirection?), static () => new APLValueConverter<ContentDirection?>() },
        { typeof(ControlMediaCommand), static () => new APLValueConverter<ControlMediaCommand>() },
        { typeof(ControlMediaCommand?), static () => new APLValueConverter<ControlMediaCommand?>() },
        { typeof(DocumentBackgroundColor), static () => new APLValueConverter<DocumentBackgroundColor>() },
        { typeof(DrawOrder), static () => new APLValueConverter<DrawOrder>() },
        { typeof(DrawOrder?), static () => new APLValueConverter<DrawOrder?>() },
        { typeof(HighlightMode), static () => new APLValueConverter<HighlightMode>() },
        { typeof(HighlightMode?), static () => new APLValueConverter<HighlightMode?>() },
        { typeof(ItemAlignment), static () => new APLValueConverter<ItemAlignment>() },
        { typeof(ItemAlignment?), static () => new APLValueConverter<ItemAlignment?>() },
        { typeof(KeyboardType), static () => new APLValueConverter<KeyboardType>() },
        { typeof(KeyboardType?), static () => new APLValueConverter<KeyboardType?>() },
        { typeof(LayoutDirection), static () => new APLValueConverter<LayoutDirection>() },
        { typeof(LayoutDirection?), static () => new APLValueConverter<LayoutDirection?>() },
        { typeof(MetadataPosition), static () => new APLValueConverter<MetadataPosition>() },
        { typeof(MetadataPosition?), static () => new APLValueConverter<MetadataPosition?>() },
        { typeof(NoiseKind), static () => new APLValueConverter<NoiseKind>() },
        { typeof(NoiseKind?), static () => new APLValueConverter<NoiseKind?>() },
        { typeof(ProgressBarType), static () => new APLValueConverter<ProgressBarType>() },
        { typeof(ProgressBarType?), static () => new APLValueConverter<ProgressBarType?>() },
        { typeof(RatingGraphicType), static () => new APLValueConverter<RatingGraphicType>() },
        { typeof(RatingGraphicType?), static () => new APLValueConverter<RatingGraphicType?>() },
        { typeof(RatingSlotMode), static () => new APLValueConverter<RatingSlotMode>() },
        { typeof(RatingSlotMode?), static () => new APLValueConverter<RatingSlotMode?>() },
        { typeof(RepeatMode), static () => new APLValueConverter<RepeatMode>() },
        { typeof(RepeatMode?), static () => new APLValueConverter<RepeatMode?>() },
        { typeof(Scale), static () => new APLValueConverter<Scale>() },
        { typeof(Scale?), static () => new APLValueConverter<Scale?>() },
        { typeof(ScrollDirection), static () => new APLValueConverter<ScrollDirection>() },
        { typeof(ScrollDirection?), static () => new APLValueConverter<ScrollDirection?>() },
        { typeof(SelectorStrategy), static () => new APLValueConverter<SelectorStrategy>() },
        { typeof(SelectorStrategy?), static () => new APLValueConverter<SelectorStrategy?>() },
        { typeof(SetPagePosition), static () => new APLValueConverter<SetPagePosition>() },
        { typeof(SetPagePosition?), static () => new APLValueConverter<SetPagePosition?>() },
        { typeof(SliderSize), static () => new APLValueConverter<SliderSize>() },
        { typeof(SliderSize?), static () => new APLValueConverter<SliderSize?>() },
        { typeof(SliderType), static () => new APLValueConverter<SliderType>() },
        { typeof(SliderType?), static () => new APLValueConverter<SliderType?>() },
        { typeof(Snap), static () => new APLValueConverter<Snap>() },
        { typeof(Snap?), static () => new APLValueConverter<Snap?>() },
        { typeof(SpeechContentType), static () => new APLValueConverter<SpeechContentType>() },
        { typeof(SpeechContentType?), static () => new APLValueConverter<SpeechContentType?>() },
        { typeof(StrokeLineCap), static () => new APLValueConverter<StrokeLineCap>() },
        { typeof(StrokeLineCap?), static () => new APLValueConverter<StrokeLineCap?>() },
        { typeof(StrokeLineJoin), static () => new APLValueConverter<StrokeLineJoin>() },
        { typeof(StrokeLineJoin?), static () => new APLValueConverter<StrokeLineJoin?>() },
        { typeof(SubmitKeyType), static () => new APLValueConverter<SubmitKeyType>() },
        { typeof(SubmitKeyType?), static () => new APLValueConverter<SubmitKeyType?>() },
        { typeof(SwipeAction), static () => new APLValueConverter<SwipeAction>() },
        { typeof(SwipeAction?), static () => new APLValueConverter<SwipeAction?>() },
        { typeof(SwipeDirection), static () => new APLValueConverter<SwipeDirection>() },
        { typeof(SwipeDirection?), static () => new APLValueConverter<SwipeDirection?>() },
        { typeof(TextOverflow), static () => new APLValueConverter<TextOverflow>() },
        { typeof(TextOverflow?), static () => new APLValueConverter<TextOverflow?>() },
        { typeof(TimeTextDirection), static () => new APLValueConverter<TimeTextDirection>() },
        { typeof(TimeTextDirection?), static () => new APLValueConverter<TimeTextDirection?>() },
        { typeof(Uri), static () => new APLValueConverter<Uri>() },
        { typeof(VideoSource), static () => new APLValueConverter<VideoSource>() },
        { typeof(bool), static () => new APLValueConverter<bool>() },
        { typeof(bool?), static () => new APLValueConverter<bool?>() },
        { typeof(double), static () => new APLValueConverter<double>() },
        { typeof(double?), static () => new APLValueConverter<double?>() },
        { typeof(int), static () => new APLValueConverter<int>() },
        { typeof(int?), static () => new APLValueConverter<int?>() },
        { typeof(object), static () => new APLValueConverter<object>() },
        { typeof(string), static () => new APLValueConverter<string>() },
    };

    public override bool CanConvert(Type typeToConvert)
    {
        if (_dimensionTypes.Contains(typeToConvert))
            return true;

        if (!typeToConvert.IsGenericType)
            return false;

        if (typeToConvert.GetGenericTypeDefinition() != typeof(APLValue<>))
            return false;

        return true;
    }

    public override System.Text.Json.Serialization.JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return _converterCache.GetOrAdd(typeToConvert, CreateConverterInternal);
    }

    private System.Text.Json.Serialization.JsonConverter? CreateConverterInternal(Type typeToConvert)
    {
        if (_dimensionTypes.Contains(typeToConvert))
        {
            return typeToConvert switch
            {
                var t when t == _aplDimensionType => new APLDimensionValueConverter(),
                var t when t == _aplAbsoluteDimensionType => new APLAbsoluteDimensionValueConverter(),
                var t when t == _aplObjectType => new APLObjectConverter(),
                _ => throw new JsonException("Should never happen!")
            };
        }

        var typeArguments = typeToConvert.GetGenericArguments();
        var valueType = typeArguments[0];

        if (_valueConverters.TryGetValue(valueType, out var factory))
        {
            return factory();
        }

        throw new NotSupportedException(
            $"APLValue<{valueType}> has no registered converter. Every APLValue<T> instantiation this " +
            "library ships must be registered in APLValueConverterFactory's closed dispatch table.");
    }
}