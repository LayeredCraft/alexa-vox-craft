using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;

namespace AlexaVoxCraft.Model.Helpers;

public static class EnumHelper
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, object>> _enumLookup = new();
    private static readonly ConcurrentDictionary<Enum, string> _enumToStringCache = new();

    public static bool TryParseEnumWithEnumMemberSupport(Type enumType, string? value, out object? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(value) || !enumType.IsEnum)
            return false;

        var map = _enumLookup.GetOrAdd(enumType, type =>
            type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToDictionary(
                    f => f.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? f.Name,
                    f => (object)Enum.Parse(type, f.Name)));

        return map.TryGetValue(value, out result);
    }

    public static bool TryParseEnumWithEnumMemberSupport<TEnum>(string? value, out TEnum result)
        where TEnum : struct, Enum
    {
        if (TryParseEnumWithEnumMemberSupport(typeof(TEnum), value, out var boxed))
        {
            result = (TEnum)boxed!;
            return true;
        }

        result = default;
        return false;
    }

    public static string ToEnumString<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return _enumToStringCache.GetOrAdd(value, enumValue =>
        {
            var enumType = typeof(TEnum);
            var name = Enum.GetName(enumType, enumValue);
            if (name == null) return string.Empty;

            var enumMember = enumType.GetField(name)?.GetCustomAttribute<EnumMemberAttribute>();
            return enumMember?.Value ?? name;
        });
    }
}