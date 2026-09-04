using AlexaVoxCraft.Model.ConnectionTasks;
using AlexaVoxCraft.Model.ConnectionTasks.Inputs;
using AlexaVoxCraft.Model.Response.Directive;

namespace AlexaVoxCraft.Model.Tests.Response;

/// <summary>
/// Component-level coverage for <see cref="StartConnectionDirective"/> built from the various
/// built-in <see cref="IConnectionTask"/> task types, plus <see cref="CompleteTaskDirective"/>. These
/// generic print/task connections are unused by the trivia skill (no real captures exist), so all
/// data is synthetic. Directives are response objects the skill only ever builds and sends, so they
/// are tested via serialize, not deserialize (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class ConnectionTaskDirectiveTests() : TestBase<ConnectionTaskDirectiveTests>
{
    [Fact]
    public async Task PrintPdfV1_Serializes_AsStartConnectionDirective()
    {
        var task = new PrintPdfV1
        {
            Title = "title",
            Description = "description",
            Url = "http://www.example.com/flywheel.pdf",
            Context = new ConnectionTaskContext { ProviderId = "your-provider-skill-id" }
        };

        var directive = task.ToConnectionDirective("none");
        directive.Uri.Should().Be(PrintPdfV1.AssociatedUri);

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "PrintPdfV1");
    }

    [Fact]
    public async Task PrintImageV1_Serializes_AsStartConnectionDirective()
    {
        var task = new PrintImageV1
        {
            Title = "Flywheel Document",
            Description = "Flywheel",
            ImageV1Type = PrintImageV1Type.JPEG,
            Url = "http://www.example.com/flywheel.jpeg"
        };

        var directive = task.ToConnectionDirective();
        directive.Uri.Should().Be(PrintImageV1.AssociatedUri);

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "PrintImageV1");
    }

    [Fact]
    public async Task PrintWebPageV1_Serializes_AsStartConnectionDirective()
    {
        var task = new PrintWebPageV1
        {
            Title = "title",
            Description = "description",
            Url = "http://www.example.com/flywheel.html"
        };

        var directive = task.ToConnectionDirective();
        directive.Uri.Should().Be(PrintWebPageV1.AssociatedUri);

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "PrintWebPageV1");
    }

    [Fact]
    public async Task ScheduleTaxiReservation_Serializes_AsStartConnectionDirective()
    {
        var task = new ScheduleTaxiReservation
        {
            PartySize = 4,
            PickupLocation = new PostalAddress
            {
                StreetAddress = "415 106th Ave NE",
                Locality = "Bellevue",
                Region = "WA",
                PostalCode = "98004",
                Country = "US"
            },
            DropoffLocation = new PostalAddress
            {
                StreetAddress = "2031 6th Ave.",
                Locality = "Seattle",
                Region = "WA",
                PostalCode = "98121",
                Country = "US"
            }
        };

        var directive = task.ToConnectionDirective();
        directive.Uri.Should().Be(ScheduleTaxiReservation.AssociatedUri);

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "ScheduleTaxiReservation");
    }

    [Fact]
    public async Task ScheduleFoodEstablishmentReservation_Serializes_AsStartConnectionDirective()
    {
        var task = new ScheduleFoodEstablishmentReservation
        {
            PartySize = 2,
            StartTime = new DateTime(2018, 04, 08, 01, 15, 46),
            Restaurant = new Restaurant
            {
                Name = "Amazon Day 1 Restaurant",
                Location = new PostalAddress
                {
                    StreetAddress = "2121 7th Avenue",
                    Locality = "Seattle",
                    Region = "WA",
                    PostalCode = "98121",
                    Country = "US"
                }
            }
        };

        var directive = task.ToConnectionDirective();
        directive.OnComplete = OnCompleteAction.ResumeSession;
        directive.Uri.Should().Be(ScheduleFoodEstablishmentReservation.AssociatedUri);

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "ScheduleFoodEstablishmentReservation");
    }

    [Fact]
    public async Task PinConfirmation_Serializes_AsStartConnectionDirective()
    {
        var task = new PinConfirmation();

        var directive = task.ToConnectionDirective("example-token");

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "PinConfirmation");
    }

    [Fact]
    public async Task CompleteTaskDirective_Serializes()
    {
        var directive = new CompleteTaskDirective(200, "return as desired");

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "CompleteTaskDirective");
    }
}
