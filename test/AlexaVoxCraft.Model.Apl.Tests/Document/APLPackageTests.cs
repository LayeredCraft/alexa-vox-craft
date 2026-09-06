using AlexaVoxCraft.Model.Apl.Package;

namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for <see cref="APLPackage"/> construction (a reusable APL package manifest, distinct
/// from an inline document). Response-side serialize only (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class APLPackageTests() : TestBase<APLPackageTests>
{
    [Fact]
    public async Task APLPackage_Serializes()
    {
        var package = new APLPackage
        {
            PackageVersion = PackageVersion.V1_0,
            Manifest = new Manifest
            {
                Id = "amzn1.apl-package.example",
                Version = "1.0.0",
                AppliesTo = "amzn1.ask.skill.example",
                PresentationDefinitions = [new PresentationDefinitionFile { Url = "https://example.com/documents/widget.json" }]
            },
            PublishingInformation = new PublishingInformation
            {
                Locales = new Dictionary<string, List<LocalePublishingInformation>>
                {
                    ["en-US"] =
                    [
                        new LocalePublishingInformation
                        {
                            TargetViewport = TargetViewport.WidgetM,
                            Metadata = new LocalePublishingInformationMetadata
                            {
                                Name = "Example Widget",
                                Description = "An example APL package",
                                Keywords = ["example"],
                                IconUri = "https://example.com/icon.png",
                                Previews = ["https://example.com/preview.png"]
                            }
                        }
                    ]
                }
            }
        };

        await TestHelper.VerifySerializedObject(package, AlexaJson, "APLPackage");
    }
}
