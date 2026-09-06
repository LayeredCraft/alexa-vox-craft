using AlexaVoxCraft.Model;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Response.Directive.Templates;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for response directives that aren't APL/ISP-specific. Directives are
/// response objects the skill only ever builds and sends, so they are tested via serialize, not
/// deserialize (see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class DirectiveTests() : TestBase<DirectiveTests>
{
    [Fact]
    public async Task HintDirective_Serializes()
    {
        var directive = new HintDirective
        {
            Hint = new Hint { Text = "sample text", Type = TextType.Plain }
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "HintDirective");
    }

    [Fact]
    public async Task DialogConfirmIntent_Serializes()
    {
        var directive = new DialogConfirmIntent
        {
            UpdatedIntent = new Intent
            {
                Name = "GetZodiacHoroscopeIntent",
                ConfirmationStatus = ConfirmationStatus.None,
                Slots = new Dictionary<string, Slot>
                {
                    ["ZodiacSign"] = new() { Name = "ZodiacSign", Value = "virgo", ConfirmationStatus = ConfirmationStatus.Confirmed },
                    ["Date"] = new() { Name = "Date", Value = "2015-11-25", ConfirmationStatus = ConfirmationStatus.Confirmed }
                }
            }
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "DialogConfirmIntent");
    }

    [Fact]
    public async Task DialogConfirmSlot_Serializes()
    {
        var directive = new DialogConfirmSlot("Date")
        {
            UpdatedIntent = new Intent
            {
                Name = "GetZodiacHoroscopeIntent",
                Slots = new Dictionary<string, Slot>
                {
                    ["ZodiacSign"] = new() { Name = "ZodiacSign", Value = "virgo" },
                    ["Date"] = new() { Name = "Date", Value = "2015-11-25", ConfirmationStatus = ConfirmationStatus.Confirmed }
                }
            }
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "DialogConfirmSlot");
    }

    [Fact]
    public async Task DialogDelegate_Serializes()
    {
        var directive = new DialogDelegate
        {
            UpdatedIntent = new Intent
            {
                Name = "GetZodiacHoroscopeIntent",
                ConfirmationStatus = ConfirmationStatus.None,
                Slots = new Dictionary<string, Slot>
                {
                    ["ZodiacSign"] = new() { Name = "ZodiacSign", Value = "virgo" },
                    ["Date"] = new() { Name = "Date", Value = "2015-11-25", ConfirmationStatus = ConfirmationStatus.Confirmed }
                }
            }
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "DialogDelegate");
    }

    [Fact]
    public async Task DialogElicitSlot_Serializes()
    {
        var directive = new DialogElicitSlot("ZodiacSign")
        {
            UpdatedIntent = new Intent
            {
                Name = "GetZodiacHoroscopeIntent",
                ConfirmationStatus = ConfirmationStatus.None,
                Slots = new Dictionary<string, Slot>
                {
                    ["ZodiacSign"] = new() { Name = "ZodiacSign", Value = "virgo" },
                    ["Date"] = new() { Name = "Date", Value = "2015-11-25", ConfirmationStatus = ConfirmationStatus.Confirmed }
                }
            }
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "DialogElicitSlot");
    }

    [Fact]
    public async Task AskForPermissionDirective_Serializes()
    {
        var directive = new AskForPermissionDirective("alexa::alerts:reminders:skill:readwrite");

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "AskForPermissionDirective");
    }

    [Fact]
    public async Task DialogUpdateDynamicEntities_Serializes()
    {
        var directive = new DialogUpdateDynamicEntities
        {
            UpdateBehavior = UpdateBehavior.Replace,
            Types =
            [
                new SlotType
                {
                    Name = "ZodiacSign",
                    Values =
                    [
                        new SlotTypeValue
                        {
                            Id = "virgo",
                            Name = new SlotTypeValueName { Value = "Virgo", Synonyms = ["The Maiden"] }
                        }
                    ]
                }
            ]
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "DialogUpdateDynamicEntities");
    }
}
