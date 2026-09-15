using System.Text.Json;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Serialization;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Scenario 2 (original console app): APL component graph round trip. APLSupport.Add() itself now runs
// once in AssemblyModuleInitializer, matching a real skill's own cold-start order (before the first
// request), rather than being called inline here.
public sealed class AplTests
{
    [Fact]
    public void Serialize_And_Deserialize_RenderDocumentDirective()
    {
        var document = new APLDocument
        {
            MainTemplate = new Layout(new Container
            {
                Items = [new Text("hello apl")]
            })
        };
        var directive = new RenderDocumentDirective(document);

        var json = JsonSerializer.Serialize<IDirective>(directive, AlexaJsonTypeInfo.For<IDirective>());

        Assert.Contains("Container", json);
        Assert.Contains("hello apl", json);

        var roundTripped = JsonSerializer.Deserialize(json, AlexaJsonTypeInfo.For<IDirective>());

        Assert.IsType<RenderDocumentDirective>(roundTripped);
    }
}
