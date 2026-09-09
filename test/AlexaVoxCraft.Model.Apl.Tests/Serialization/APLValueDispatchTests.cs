using System.Text.Json;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests.Serialization;

/// <summary>
/// Proves APLValueConverterFactory/APLValueCollectionConverterFactory's closed dispatch tables (Task
/// Group 4) actually produce a converter for every APLValue&lt;T&gt;/APLValueCollection&lt;T&gt;
/// instantiation this library ships, and fail loudly - not silently - for one that isn't registered.
/// </summary>
public class APLValueDispatchTests
{
    private static readonly APLValueConverterFactory ValueFactory = new();
    private static readonly APLValueCollectionConverterFactory CollectionFactory = new();

    [Theory]
    [InlineData(typeof(APLValue<string>))]
    [InlineData(typeof(APLValue<int>))]
    [InlineData(typeof(APLValue<int?>))]
    [InlineData(typeof(APLValue<bool>))]
    [InlineData(typeof(APLValue<bool?>))]
    [InlineData(typeof(APLValue<Scale>))]
    [InlineData(typeof(APLValue<Scale?>))]
    [InlineData(typeof(APLValue<Uri>))]
    [InlineData(typeof(APLValue<object>))]
    [InlineData(typeof(APLValue<APLComponent>))]
    [InlineData(typeof(APLValueCollection<APLCommand>))]
    [InlineData(typeof(APLValueCollection<string>))]
    [InlineData(typeof(APLValueCollection<int>))]
    [InlineData(typeof(APLValueCollection<APLValue<int?>>))]
    public void ClosedTypes_ResolveViaFactory_WithoutReflection(Type closedType)
    {
        var factory = closedType.GetGenericTypeDefinition() == typeof(APLValueCollection<>)
            ? (JsonConverterFactory)CollectionFactory
            : ValueFactory;

        factory.CanConvert(closedType).Should().BeTrue();
        var converter = factory.CreateConverter(closedType, AlexaJsonOptions.DefaultOptions);
        converter.Should().NotBeNull();
    }

    [Fact]
    public void APLValueConverterFactory_UnregisteredType_FailsLoudly()
    {
        var act = () => ValueFactory.CreateConverter(typeof(APLValue<UnregisteredAplValueType>), AlexaJsonOptions.DefaultOptions);

        act.Should().Throw<NotSupportedException>().WithMessage("*UnregisteredAplValueType*");
    }

    [Fact]
    public void APLValueCollectionConverterFactory_UnregisteredType_FailsLoudly()
    {
        var act = () => CollectionFactory.CreateConverter(typeof(APLValueCollection<UnregisteredAplValueType>), AlexaJsonOptions.DefaultOptions);

        act.Should().Throw<NotSupportedException>().WithMessage("*UnregisteredAplValueType*");
    }
}

public sealed class UnregisteredAplValueType
{
    public string? Value { get; set; }
}
