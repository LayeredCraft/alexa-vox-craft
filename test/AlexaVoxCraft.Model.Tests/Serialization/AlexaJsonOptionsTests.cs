using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Directive.Templates;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Tests.Serialization;

/// <summary>
/// AlexaJsonOptions is process-global mutable state (RegisterConverter/RegisterTypeModifier/
/// RegisterTypeInfoResolver/RegisterPackageTypeInfoResolver all mutate static fields). Tests that call
/// these must not run in parallel with each other or with anything reading DefaultOptions, hence a
/// dedicated non-parallel collection.
/// </summary>
[CollectionDefinition(nameof(AlexaJsonOptionsCollection), DisableParallelization = true)]
public sealed class AlexaJsonOptionsCollection;

[Collection(nameof(AlexaJsonOptionsCollection))]
public sealed class AlexaJsonOptionsTests
{
    [Fact]
    public void ResponseBody_Directives_ShouldSerialize_OnlyWhenNonEmpty()
    {
        var withDirectives = new ResponseBody { Directives = [new JsonDirective()] };
        var withoutDirectives = new ResponseBody { Directives = [] };

        var jsonWith = JsonSerializer.Serialize(withDirectives, AlexaJsonOptions.DefaultOptions);
        var jsonWithout = JsonSerializer.Serialize(withoutDirectives, AlexaJsonOptions.DefaultOptions);

        jsonWith.Should().Contain("\"directives\"");
        jsonWithout.Should().NotContain("\"directives\"");
    }

    [Fact]
    public void Reprompt_Directives_ShouldSerialize_OnlyWhenNonEmpty()
    {
        var withDirectives = new Reprompt("hi") { Directives = [new JsonDirective()] };
        var withoutDirectives = new Reprompt("hi") { Directives = [] };

        var jsonWith = JsonSerializer.Serialize(withDirectives, AlexaJsonOptions.DefaultOptions);
        var jsonWithout = JsonSerializer.Serialize(withoutDirectives, AlexaJsonOptions.DefaultOptions);

        jsonWith.Should().Contain("\"directives\"");
        jsonWithout.Should().NotContain("\"directives\"");
    }

    [Theory]
    [InlineData(0, 0, false, false)]
    [InlineData(640, 480, true, true)]
    [InlineData(0, 480, false, true)]
    public void ImageSource_WidthHeight_ShouldSerialize_OnlyWhenPositive(int width, int height, bool expectWidth, bool expectHeight)
    {
        var imageSource = new ImageSource { Url = "https://example.com/img.png", Width = width, Height = height };

        var json = JsonSerializer.Serialize(imageSource, AlexaJsonOptions.DefaultOptions);

        if (expectWidth) json.Should().Contain("\"widthPixels\"");
        else json.Should().NotContain("\"widthPixels\"");

        if (expectHeight) json.Should().Contain("\"heightPixels\"");
        else json.Should().NotContain("\"heightPixels\"");
    }

    [Fact]
    public void JitFallback_ResolvesUnregisteredPoco_WhenReflectionEnabled()
    {
        var poco = new UnregisteredJitOnlyPoco { Value = "hello" };

        var json = JsonSerializer.Serialize(poco, AlexaJsonOptions.DefaultOptions);
        var roundTripped = JsonSerializer.Deserialize<UnregisteredJitOnlyPoco>(json, AlexaJsonOptions.DefaultOptions);

        roundTripped.Should().NotBeNull();
        roundTripped!.Value.Should().Be("hello");
    }

    [Fact]
    public void RegisterTypeInfoResolver_ConsumerContext_IsUsedByDefaultOptions()
    {
        AlexaJsonOptions.RegisterTypeInfoResolver(ConsumerOwnedContext.Default);

        var poco = new ConsumerOwnedPoco { Name = "registered" };
        var json = JsonSerializer.Serialize(poco, AlexaJsonOptions.DefaultOptions);
        var roundTripped = JsonSerializer.Deserialize<ConsumerOwnedPoco>(json, AlexaJsonOptions.DefaultOptions);

        roundTripped.Should().NotBeNull();
        roundTripped!.Name.Should().Be("registered");
    }

    [Fact]
    public void RegisterPackageTypeInfoResolver_PrecedesConsumerResolver_RegardlessOfCallOrder()
    {
        // Register consumer resolver first, package resolver second - the invariant is that package
        // resolvers always precede consumer resolvers in the composed chain, independent of this order.
        AlexaJsonOptions.RegisterTypeInfoResolver(ConsumerOwnedContext.Default);
        AlexaJsonOptions.RegisterPackageTypeInfoResolver(PackageOwnedContext.Default);

        var packagePoco = new PackageOwnedPoco { Value = 42 };
        var json = JsonSerializer.Serialize(packagePoco, AlexaJsonOptions.DefaultOptions);
        var roundTripped = JsonSerializer.Deserialize<PackageOwnedPoco>(json, AlexaJsonOptions.DefaultOptions);

        roundTripped.Should().NotBeNull();
        roundTripped!.Value.Should().Be(42);
    }

    [Fact]
    public void RegisterConverter_TakesEffect_WhenRegisteredBeforeFirstAccess()
    {
        AlexaJsonOptions.RegisterConverter(new BeforeAccessConverter());

        var value = new BeforeAccessType { Value = "x" };
        var json = JsonSerializer.Serialize(value, AlexaJsonOptions.DefaultOptions);

        json.Should().Be("\"custom:x\"");
    }

    [Fact]
    public void RegisterConverter_TakesEffect_WhenRegisteredAfterFirstAccess()
    {
        // Force first access/cache population before registering.
        _ = AlexaJsonOptions.DefaultOptions;

        AlexaJsonOptions.RegisterConverter(new AfterAccessConverter());

        var value = new AfterAccessType { Value = "y" };
        var json = JsonSerializer.Serialize(value, AlexaJsonOptions.DefaultOptions);

        json.Should().Be("\"custom:y\"");
    }

    [Fact]
    public void RegisterTypeModifier_TakesEffect_WhenRegisteredBeforeFirstAccess()
    {
        AlexaJsonOptions.RegisterTypeModifier<BeforeAccessModifierType>(ti =>
        {
            var prop = ti.Properties.FirstOrDefault(p => p.Name == "Value");
            prop?.ShouldSerialize = (_, _) => false;
        });

        var value = new BeforeAccessModifierType { Value = "should-be-omitted" };
        var json = JsonSerializer.Serialize(value, AlexaJsonOptions.DefaultOptions);

        json.Should().NotContain("should-be-omitted");
    }

    [Fact]
    public void RegisterTypeModifier_TakesEffect_WhenRegisteredAfterFirstAccess()
    {
        _ = AlexaJsonOptions.DefaultOptions;

        AlexaJsonOptions.RegisterTypeModifier<AfterAccessModifierType>(ti =>
        {
            var prop = ti.Properties.FirstOrDefault(p => p.Name == "Value");
            prop?.ShouldSerialize = (_, _) => false;
        });

        var value = new AfterAccessModifierType { Value = "should-be-omitted" };
        var json = JsonSerializer.Serialize(value, AlexaJsonOptions.DefaultOptions);

        json.Should().NotContain("should-be-omitted");
    }
}

public sealed class UnregisteredJitOnlyPoco
{
    public string? Value { get; set; }
}

public sealed class ConsumerOwnedPoco
{
    public string? Name { get; set; }
}

[JsonSerializable(typeof(ConsumerOwnedPoco))]
internal partial class ConsumerOwnedContext : JsonSerializerContext;

public sealed class PackageOwnedPoco
{
    public int Value { get; set; }
}

[JsonSerializable(typeof(PackageOwnedPoco))]
internal partial class PackageOwnedContext : JsonSerializerContext;

public sealed class BeforeAccessType
{
    public string? Value { get; set; }
}

public sealed class BeforeAccessConverter : JsonConverter<BeforeAccessType>
{
    public override BeforeAccessType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, BeforeAccessType value, JsonSerializerOptions options)
        => writer.WriteStringValue($"custom:{value.Value}");
}

public sealed class AfterAccessType
{
    public string? Value { get; set; }
}

public sealed class AfterAccessConverter : JsonConverter<AfterAccessType>
{
    public override AfterAccessType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, AfterAccessType value, JsonSerializerOptions options)
        => writer.WriteStringValue($"custom:{value.Value}");
}

public sealed class BeforeAccessModifierType
{
    public string? Value { get; set; }
}

public sealed class AfterAccessModifierType
{
    public string? Value { get; set; }
}
