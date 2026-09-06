using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for <see cref="APLDocument"/> construction. Documents are response objects the skill
/// only ever builds and sends, so they are tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class APLDocumentTests() : TestBase<APLDocumentTests>
{
    [Theory]
    [InlineData(APLDocumentVersion.V1, "1.0")]
    [InlineData(APLDocumentVersion.V1_1, "1.1")]
    [InlineData(APLDocumentVersion.V1_2, "1.2")]
    [InlineData(APLDocumentVersion.V1_3, "1.3")]
    [InlineData(APLDocumentVersion.V1_4, "1.4")]
    [InlineData(APLDocumentVersion.V1_5, "1.5")]
    [InlineData(APLDocumentVersion.V1_6, "1.6")]
    [InlineData(APLDocumentVersion.V1_7, "1.7")]
    [InlineData(APLDocumentVersion.V1_8, "1.8")]
    [InlineData(APLDocumentVersion.V1_9, "1.9")]
    [InlineData(APLDocumentVersion.V2022_1, "2022.1")]
    [InlineData(APLDocumentVersion.V2022_2, "2022.2")]
    [InlineData(APLDocumentVersion.V2023_1, "2023.1")]
    [InlineData(APLDocumentVersion.V2023_2, "2023.2")]
    [InlineData(APLDocumentVersion.V2024_1, "2024.1")]
    public void Version_SerializesToExpectedVersionString(APLDocumentVersion version, string expected)
    {
        var document = new APLDocument(version) { MainTemplate = new Layout(new Text { Content = "Question text" }) };

        document.VersionString.Should().Be(expected);
    }

    [Fact]
    public async Task Document_Serializes_WithResourcesStylesAndImports()
    {
        var document = new APLDocument(APLDocumentVersion.V1_6)
        {
            MainTemplate = new Layout(new Text { Content = "Question text" }),
            Imports = [Import.AlexaLayouts],
            Resources =
            [
                new Resource
                {
                    Dimensions = new Dictionary<string, APLDimensionValue> { ["myFontSize"] = "28dp" }
                },
                new Resource("${@viewportProfile == @hubRound}")
                {
                    Colors = new Dictionary<string, string> { ["highlightColor"] = "#FFFFFF" }
                }
            ],
            Styles = new Dictionary<string, Style>
            {
                ["baseText"] = new()
                {
                    Values =
                    [
                        new StyleValue(new Dictionary<string, object> { ["fontFamily"] = "Amazon Ember Display" }),
                        new StyleValue(new Dictionary<string, object> { ["color"] = "blue" }) { When = "${state.focused}" }
                    ]
                },
                ["title"] = new()
                {
                    Extends = ["baseText"],
                    Values = [new StyleValue(new Dictionary<string, object> { ["fontWeight"] = "bold" })]
                }
            }
        };

        await TestHelper.VerifySerializedObject(document, AlexaJson, "Document_WithResourcesStylesAndImports");
    }

    [Fact]
    public async Task Document_Serializes_WithLifecycleHooksAndSettings()
    {
        var document = new APLDocument(APLDocumentVersion.V1_6)
        {
            MainTemplate = new Layout(new Text { Content = "Question text" }),
            Settings = new APLDocumentSettings { SupportsResizing = true },
            OnMount = [new SetValue { ComponentId = "root", Property = "opacity"!, Value = "1"! }],
            OnConfigChange = [new Reinflate()]
        };

        await TestHelper.VerifySerializedObject(document, AlexaJson, "Document_WithLifecycleHooksAndSettings");
    }

    [Fact]
    public async Task Import_Serializes()
    {
        var import = new Import("alexa-styles", "1.0.0");

        await TestHelper.VerifySerializedObject(import, AlexaJson, "Import");
    }

    [Fact]
    public async Task DocumentLink_Serializes()
    {
        var link = new APLDocumentLink("https://example.com/documents/my-document.json");

        await TestHelper.VerifySerializedObject(link, AlexaJson, "APLDocumentLink");
    }

    [Fact]
    public void Import_Into_AddsImportWhenAbsent()
    {
        var document = new APLDocument();

        Import.AlexaLayouts.Into(document);

        document.Imports.Should().ContainSingle().Which.Should().BeEquivalentTo(Import.AlexaLayouts);
    }

    [Fact]
    public void Import_Into_DoesNotDuplicateExistingImport()
    {
        var document = new APLDocument { Imports = [Import.AlexaLayouts] };

        Import.AlexaLayouts.Into(document);

        document.Imports.Should().ContainSingle();
    }
}
