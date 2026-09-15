using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.NativeAot.AotTests;

/// <summary>
/// Resolves a <see cref="JsonTypeInfo{T}"/> from <see cref="AlexaJsonOptions.DefaultOptions"/>'s own
/// resolver chain (round 12 investigation, LayeredCraft/compono#140-era conversion) - the AOT/trim
/// analyzer flags every <c>JsonSerializer.Serialize&lt;T&gt;(value, JsonSerializerOptions)</c>/
/// <c>Deserialize&lt;T&gt;(json, JsonSerializerOptions)</c> call as IL2026/IL3050 regardless of
/// whether the options' resolver actually covers <typeparamref name="T"/>, because it can't prove
/// that statically - the same reason <c>AlexaLambdaSerializer</c> (this library's own production
/// code) carries the identical warning at its own call sites. <see cref="JsonSerializerOptions.GetTypeInfo(Type)"/>
/// itself is NOT so annotated (confirmed by direct probe) - it's the sanctioned way to pull a
/// <see cref="JsonTypeInfo{T}"/> out of an already-configured, already-resolver-covered options
/// instance, then use one of <c>JsonSerializer</c>'s <see cref="JsonTypeInfo{T}"/>-accepting
/// overloads instead of the ambient-<c>JsonSerializerOptions</c> ones. This models the actual
/// AOT-safe consumer path more precisely than the generic overload does, and removes the warning
/// without a suppression - the test still proves the exact same thing (the type round-trips /
/// resolves correctly through <see cref="AlexaJsonOptions.DefaultOptions"/>'s real resolver chain),
/// just through the call shape a genuinely AOT-safe consumer would actually use.
/// </summary>
internal static class AlexaJsonTypeInfo
{
    public static JsonTypeInfo<T> For<T>() => (JsonTypeInfo<T>)AlexaJsonOptions.DefaultOptions.GetTypeInfo(typeof(T));
}
