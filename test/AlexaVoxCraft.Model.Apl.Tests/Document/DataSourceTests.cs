using System.Text.Json;
using AlexaVoxCraft.Model.Apl.DataSources;

namespace AlexaVoxCraft.Model.Apl.Tests.Document;

/// <summary>
/// Coverage for the paginated/dynamic data source types sent alongside a
/// <see cref="RenderDocumentDirective"/>. Response-side serialize only (see
/// docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md).
/// </summary>
public sealed class DataSourceTests() : TestBase<DataSourceTests>
{
    [Fact]
    public async Task ListDataSource_Serializes()
    {
        var list = new ListDataSource { ListId = "lt1Sample", TotalNumberOfItems = 3 };
        list.ListPage.ListItems.Add(new { listItemIdentifier = "gouda", token = "gouda", ordinalNumber = 1 });
        list.ListPage.ListItems.Add(new { listItemIdentifier = "cheddar", token = "cheddar", ordinalNumber = 2 });
        list.ListPage.ListItems.Add(new { listItemIdentifier = "brie", token = "brie", ordinalNumber = 3 });

        await TestHelper.VerifySerializedObject(list, AlexaJson, "ListDataSource");
    }

    [Fact]
    public async Task DynamicIndexList_Serializes()
    {
        var list = new DynamicIndexList("my-list-id", 0) { MinimumInclusiveIndex = 0, MaximumExclusiveIndex = 200 };
        list.Items!.Add(new { primaryText = "item 1" });
        list.Items.Add(new { primaryText = "item 2" });
        list.Items.Add(new { primaryText = "item 3" });

        await TestHelper.VerifySerializedObject(list, AlexaJson, "DynamicIndexList");
    }

    [Fact]
    public async Task DynamicTokenList_Serializes()
    {
        var list = new DynamicTokenList
        {
            ListId = "my-list-id",
            PageToken = "initial-token",
            ForwardPageToken = "forward-token"
        };
        list.Items!.Add(new { primaryText = "item 1" });
        list.Items.Add(new { primaryText = "item 2" });

        await TestHelper.VerifySerializedObject(list, AlexaJson, "DynamicTokenList");
    }

    [Fact]
    public async Task KeyValueDataSource_Serializes_WithoutTypeWrapper()
    {
        var source = new KeyValueDataSource
        {
            Properties = new Dictionary<string, JsonElement>
            {
                ["headerTitle"] = JsonSerializer.SerializeToElement("Leaderboard"),
                ["listItemsToShow"] = JsonSerializer.SerializeToElement(new[]
                {
                    new { primaryText = "Player1", tertiaryText = "100" }
                })
            }
        };

        await TestHelper.VerifySerializedObject(source, AlexaJson, "KeyValueDataSource");
    }
}
