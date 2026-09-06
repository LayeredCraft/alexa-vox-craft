using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;

namespace AlexaVoxCraft.Model.Apl.Tests.Components;

/// <summary>
/// Component-level coverage for the Alexa list family (AlexaImageList/AlexaLists/
/// AlexaPaginatedList/AlexaGridList and the AlexaImageListItem list-item shape).
/// </summary>
public class AlexaListTests : TestBase<AlexaListTests>
{
    [Fact]
    public async Task AlexaImageListItem_Serializes()
    {
        var item = new AlexaImageListItem
        {
            Theme = "dark",
            PrimaryText = "${exampleData.primaryText}",
            SecondaryText = "${exampleData.secondaryText}"!,
            TertiaryText = "${exampleData.tertiaryText}"!,
            ProviderText = "${exampleData.providerText}",
            ImageProgressBarPercentage = 75,
            ImageRoundedCorner = true,
            ImageAspectRatio = AlexaImageAspectRatio.Square,
            ImageSource = "${exampleData.imageSource}"
        };

        await TestHelper.VerifySerializedObject(item, AlexaJson, "AlexaImageListItem");
    }

    [Fact]
    public async Task AlexaImageList_Serializes()
    {
        var control = new AlexaImageList
        {
            ListItems = "${imageListData.listItemsToShow}",
            DefaultImageSource = "https://d2o906d8ln7ui1.cloudfront.net/images/BT7_Background.png",
            ImageBlurredBackground = true,
            PrimaryAction = [new SendEvent { Arguments = [(object)"ListItemSelected", "${ordinal}"] }]
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaImageList");
    }

    [Fact]
    public async Task AlexaLists_Serializes()
    {
        var control = new AlexaLists
        {
            ListItems = "${listData.listItemsToShow}",
            ListImagePrimacy = true,
            DefaultImageSource = "https://d2o906d8ln7ui1.cloudfront.net/images/BT7_Background.png",
            ImageBlurredBackground = true
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaLists");
    }

    [Fact]
    public async Task AlexaPaginatedList_Serializes()
    {
        var control = new AlexaPaginatedList
        {
            ListId = "myPaginatedList",
            HeaderAttributionOpacity = 1.0,
            SpeechItems = "${speechData.items}"
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaPaginatedList");
    }

    [Fact]
    public async Task AlexaGridList_Serializes()
    {
        var control = new AlexaGridList
        {
            CustomLayoutName = "MyGridLayout",
            DefaultImnageSource = "https://d2o906d8ln7ui1.cloudfront.net/images/BT7_Background.png",
            ImageAspectRatio = AlexaImageAspectRatio.Square,
            ListItemHorizontalCount = 3,
            HideOrdinal = true
        };

        await TestHelper.VerifySerializedObject(control, AlexaJson, "AlexaGridList");
    }
}
