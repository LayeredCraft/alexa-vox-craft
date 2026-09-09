using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Http.Serialization;

/// <summary>
/// Forwards every <see cref="GetTypeInfo"/> call to whatever <see cref="AlexaJsonOptions.DefaultOptions"/>'s
/// <see cref="JsonSerializerOptions.TypeInfoResolver"/> currently is, rather than capturing it once. This
/// lets an outer <see cref="JsonSerializerOptions"/> instance be constructed once (and safely locked by
/// System.Text.Json on first use) while still observing consumer resolver registrations
/// (<see cref="AlexaJsonOptions.RegisterTypeInfoResolver"/>) made after that instance was constructed but
/// before its first actual use - the resolver-freshness invariant AlexaVoxCraft.Http's and
/// AlexaVoxCraft.Smapi's default serialization paths must satisfy (see plan 0003, Task Group 3d).
/// </summary>
internal sealed class DelegatingModelTypeInfoResolver : IJsonTypeInfoResolver
{
    public static readonly DelegatingModelTypeInfoResolver Instance = new();

    private DelegatingModelTypeInfoResolver()
    {
    }

    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
        => AlexaJsonOptions.DefaultOptions.TypeInfoResolver?.GetTypeInfo(type, options);
}
