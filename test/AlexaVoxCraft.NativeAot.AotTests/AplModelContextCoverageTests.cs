using System.Text.Json;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Response;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Issue #198: AplModelContext's coverage strategy (RegisterTypeInfo<T>() calls + polymorphic-converter
// derived-type targets) only reached component/command *classes*. It had no equivalent path for the
// enum types those classes expose through APLValue<T>-wrapped, object-typed properties - APLValue<T>.Write
// and SingleOrListConverter<T>.Write both erase to object before calling JsonSerializer.Serialize, which
// resolves JsonTypeInfo by the value's *runtime* type, not any statically-declared one. Each case below
// reproduces one of the real failures found serializing an actual deployed Native AOT Lambda response,
// using AlexaJsonOptions.DefaultOptions - the real resolver chain, reflection disabled
// (JsonSerializerIsReflectionEnabledByDefault=false, set on this project) - with no consumer-side
// RegisterTypeInfoResolver call, proving both that the gap was AlexaVoxCraft's own metadata (not a
// missing consumer registration) and that AplModelContext's fix resolves it.
public sealed class AplModelContextCoverageTests
{
    [Fact]
    public void Serialize_Image_WithScale_RoundTrips()
    {
        var image = new Image { Scale = AlexaVoxCraft.Model.Apl.Components.Scale.BestFill };

        var json = JsonSerializer.Serialize<APLComponent>(image, AlexaJsonTypeInfo.For<APLComponent>());

        Assert.Contains("\"best-fill\"", json);
    }

    [Fact]
    public void Serialize_SetPage_WithSetPagePosition_RoundTrips()
    {
        var command = new SetPage { Value = 1, Position = SetPagePosition.Relative };

        var json = JsonSerializer.Serialize<APLCommand>(command, AlexaJsonTypeInfo.For<APLCommand>());

        Assert.Contains("\"relative\"", json);
    }

    [Fact]
    public void Serialize_Component_WithExplicitDisplay_RoundTrips()
    {
        // APLDisplay lives on APLComponent itself, not a specific component subclass - any component
        // on a document that sets it (or any document rendered for a device whose supportedInterfaces
        // causes a Display value to be written) hits this, regardless of which components it uses.
        var text = new Text("hi") { Display = APLDisplay.Normal };

        var json = JsonSerializer.Serialize<APLComponent>(text, AlexaJsonTypeInfo.For<APLComponent>());

        Assert.Contains("\"normal\"", json);
    }

    [Fact]
    public void Serialize_RenderDocumentDirective_WithScale_RoundTrips()
    {
        // End-to-end version of the same gap, through the real directive/document graph the Lambda
        // response serializer actually walks (AplTests.Serialize_And_Deserialize_RenderDocumentDirective
        // covers the same graph shape but never sets an APLValue<TEnum> property, which is why it passed
        // while real deployed traffic did not).
        var document = new APLDocument
        {
            MainTemplate = new Layout(new Image { Scale = AlexaVoxCraft.Model.Apl.Components.Scale.BestFill })
        };
        var directive = new RenderDocumentDirective(document);

        var json = JsonSerializer.Serialize<IDirective>(directive, AlexaJsonTypeInfo.For<IDirective>());

        Assert.Contains("\"best-fill\"", json);
    }

    [Fact]
    public void Serialize_CustomComponent_WithListOfAPLComponentInProperties_RoundTrips()
    {
        // The originally-reported List<APLComponent> gap: CustomComponent (an AlexaVoxCraft-owned type,
        // meant precisely for consumers to add vendor-extension properties) inherits APLComponent's
        // [JsonExtensionData] Dictionary<string, object> Properties bag. Nesting real APLComponent
        // objects under a custom property key - the documented way to build a custom container-like
        // component - stores them as a raw List<APLComponent>, which never goes through
        // APLValueCollection<T> (that's Container/Pager/Sequence/etc.'s own Items, not this). The
        // dictionary value is object-erased the same way APLValue<T>/APLValueCollection<T> are, so it
        // needs the same kind of explicit root - but List<APLComponent> isn't a key in either factory's
        // dispatch table (this isn't an APLValue<T>/APLValueCollection<T> instantiation), so
        // AplModelContextCompletenessTests' reflection-based audit can't find this one; it has to be
        // covered by this direct repro instead.
        var custom = new CustomComponent("CustomContainer")
        {
            Properties = new Dictionary<string, object>
            {
                ["items"] = new List<APLComponent> { new Text("nested") }
            }
        };

        var json = JsonSerializer.Serialize<APLComponent>(custom, AlexaJsonTypeInfo.For<APLComponent>());

        Assert.Contains("\"nested\"", json);
    }
}

