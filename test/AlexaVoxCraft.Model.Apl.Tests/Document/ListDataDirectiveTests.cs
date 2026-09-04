using AlexaVoxCraft.Model.Apl.Operation;

namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for the dynamic-list-management directives (<see cref="SendIndexListDataDirective"/>,
/// <see cref="SendTokenListDataDirective"/>, <see cref="UpdateIndexListDataDirective"/>) and the
/// <see cref="Operation"/> types the update directive carries. Response-side serialize only (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class ListDataDirectiveTests() : TestBase<ListDataDirectiveTests>
{
    [Fact]
    public async Task SendIndexListDataDirective_Serializes()
    {
        var directive = new SendIndexListDataDirective
        {
            Token = "developer-provided-token",
            CorrelationToken = "alexa-provided-correlation-token",
            ListId = "my-list-id",
            ListVersion = 3,
            StartIndex = 11,
            MinimumInclusiveIndex = 11,
            MaximumExclusiveIndex = 21,
            Items = [new { primaryText = "item 11" }, new { primaryText = "item 12" }, new { primaryText = "item 13" }]
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "SendIndexListDataDirective");
    }

    [Fact]
    public async Task SendTokenListDataDirective_Serializes()
    {
        var directive = new SendTokenListDataDirective
        {
            Token = "developer-provided-token",
            CorrelationToken = "alexa-provided-correlation-token",
            ListId = "my-list-id",
            PageToken = "current-page-token",
            NextPageToken = "next-page-token",
            Items = [new { primaryText = "item 1" }, new { primaryText = "item 2" }]
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "SendTokenListDataDirective");
    }

    [Fact]
    public async Task UpdateIndexListDataDirective_Serializes_WithAllOperationTypes()
    {
        var directive = new UpdateIndexListDataDirective
        {
            Token = "developer-provided-token",
            ListId = "my-list-id",
            ListVersion = 4,
            Operations =
            [
                new InsertItem(10, new { primaryText = "inserted" }),
                new InsertMultipleItems(12, new { primaryText = "a" }, new { primaryText = "b" }, new { primaryText = "c" }),
                new SetItem(14, new { primaryText = "replaced" }),
                new DeleteItem(16),
                new DeleteMultipleItems(17, 2)
            ]
        };

        await TestHelper.VerifySerializedObject(directive, AlexaJson, "UpdateIndexListDataDirective");
    }
}
