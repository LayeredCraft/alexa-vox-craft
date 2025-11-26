using System.Text.Json;
using AlexaVoxCraft.Model.Apl.Commands;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Apl.DataSources;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AlexaVoxCraft.Model.Apl.Legacy.Tests;

public class ComponentTests
{
    private readonly ITestOutputHelper _output;

    public ComponentTests(ITestOutputHelper output)
    {
        _output = output;
    }
        
    [Fact]
    public void ComponentTypes()
    {
        var result = GenerateComponent("Container");
        Assert.IsType<Container>(result);

        result = GenerateComponent("Text");
        Assert.IsType<Text>(result);


    }

    [Fact]
    public void Bindings()
    {
        var component = Utility.ExampleFileContent<Text>("Binding.json");
        component.When = APLValue.To<bool?>("${@viewportProfile == @hubLandscapeSmall}");
        Assert.Equal(2, component.Bindings.Count);

        var first = component.Bindings.First();
        Assert.Equal("foo", first.Name);
        Assert.Equal("27", first.Value);

        var second = component.Bindings.Skip(1).First();
        Assert.Equal("bar", second.Name);
        Assert.Equal("${foo + 23}", second.Value);
            
    }

    [Fact]
    public void RandomClassTest()
    {
        var component = GenerateComponent("random");
        Assert.IsType<CustomComponent>(component);
        Assert.Single(((CustomComponent)component).Properties);
    }

    [Fact]
    public void VideoComponent()
    {
        Utility.AssertComponent<Video>("Video.json", _output);
    }

    [Fact]
    public void AalmadaTest()
    {
        var response = ResponseBuilder
            .Ask("Welcome to my skill. How can I help", new Reprompt());

        // Serialize the APLDocument to JsonElement
        var aplDocument = new APLDocument(APLDocumentVersion.V1_2);
        var aplElement = JsonSerializer.SerializeToElement(aplDocument, AlexaJsonOptions.DefaultOptions);

        // Build the data source
        var launchTemplateData = new ObjectDataSource
        {
            ObjectId = "launchScreen",
            Properties = new Dictionary<string, object>
            {
                { "textContent", "My Skill" },
                { "hintText", "Try, \"What can you do?\"" }
            },
            TopLevelData = new Dictionary<string, object>()
        };
        // Create the directive
        var directive = CreateAplDirective(aplElement, ("launchTemplateData", launchTemplateData));

        response.Response.Directives.Add(directive);

        // Serialize the response with System.Text.Json
        var json = JsonSerializer.Serialize(response, AlexaJsonOptions.DefaultOptions);
                
        // 👇 Optional: Output the JSON in test output
        _output.WriteLine(json);
    }

    static JsonDirective CreateAplDirective(JsonElement apl, params (string Key, ObjectDataSource Value)[] dataSources)
    {
        var directive = new JsonDirective(RenderDocumentDirective.APLDirectiveType);

        directive.Properties.Add("document", apl);

        // Build the dataSources dictionary
        var sources = new Dictionary<string, ObjectDataSource>();
        foreach (var (key, dataSource) in dataSources)
            sources.Add(key, dataSource);
            
        // Serialize dataSources to JsonElement
        var dataSourcesElement = JsonSerializer.SerializeToElement(sources, AlexaJsonOptions.DefaultOptions);
        directive.Properties.Add("datasources", dataSourcesElement);

        return directive;
    }

    [Fact]
    public void ContainerTest()
    {
        new Container
        (
            new Text("APL in C#")
            {
                FontSize = "24dp",
                TextAlign = "Center",
            },
            new Image(
                "https://images.example.com/photos/2143/lights-party-dancing-music.jpg?cs=srgb&dl=cheerful-club-concert-2143.jpg&fm=jpg")
            {
                Width = 400,
                Height = 400,
            }
        );
    }

    [Fact]
    public void APLComponentValue()
    {
        var text = new Text("Hello World")
        {
            Color = APLValue.To<string>("${color}"),
            Disabled = APLValue.To<bool?>("${disabled}"),
            FontSize = "24dp",
            Left = new AbsoluteDimension(24, "vw"),
            PaddingLeft = new RelativeDimension(5),
            Top = "${top}",
            Right = new APLDimensionValue(new AbsoluteDimension(345, "dp")),
            Bottom = new APLDimensionValue("test")
        };

        var json = JsonSerializer.Serialize(text, AlexaJsonOptions.DefaultOptions);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("24dp", root.GetProperty("fontSize").GetString());
        Assert.Equal("24vw", root.GetProperty("left").GetString());
        Assert.Equal("5%", root.GetProperty("paddingLeft").GetString());
        Assert.Equal("${top}", root.GetProperty("top").GetString());
        Assert.Equal("345dp", root.GetProperty("right").GetString());
        Assert.Equal("test", root.GetProperty("bottom").GetString());
    }

    [Fact]
    public void ImageFilters()
    {
        Utility.AssertComponent<Image>("ImageFilters.json", _output);
    }

    [Fact]
    public void TimeText()
    {
        var timeText = new TimeText
        {
            Direction = TimeTextDirection.Down,
            Format = "%M:%S",
            Start = 1552070232
        };

        Assert.True(Utility.CompareJson(timeText, "TimeText.json", _output));
    }



    [Fact]
    public void KeyboardEvent()
    {
        Utility.AssertComponent<APLComponent>("KeyboardTouchWrapper.json", _output);
    }

    [Fact]
    public void AlexaIconButton()
    {
        var control = new AlexaIconButton
        {
            ButtonSize = new AbsoluteDimension(72, "dp"),
            VectorSource =
                "M21.343,8.661l-7.895-7.105c-0.823-0.741-2.073-0.741-2.896,0L2.657,8.661C2.238,9.039,2,9.564,2,10.113V20c0,1.105,0.943,2,2.105,2H9v-9h6v9h4.895C21.057,22,22,21.105,22,20v-9.887C22,9.564,21.762,9.039,21.343,8.661z",
            PrimaryAction = new APLCommand[]{new SetValue
            {
                ComponentId = "textToUpdate",
                Property = "text",
                Value = APLValue.To<string>("${exampleData.imageStyleText}")
            }}.ToList()
        };
        Assert.True(Utility.CompareJson(control,"AlexaIconButton.json", _output));
    }

    [Fact]
    public void AlexaImageListItem()
    {
        var control = new AlexaImageListItem
        {
            Theme = "dark",
            PrimaryText = "${exampleData.primaryText}",
            SecondaryText = "${exampleData.secondaryText}",
            TertiaryText = "${exampleData.tertiaryText}",
            ProviderText = "${exampleData.providerText}",
            ImageProgressBarPercentage = 75,
            ImageRoundedCorner = true,
            ImageAspectRatio = AlexaImageAspectRatio.Square,
            ImageSource = "${exampleData.imageSource}"
        };
        Assert.True(Utility.CompareJson(control,"AlexaImageListItem.json", _output));
    }


    [Fact]
    public void AlexaRating()
    {
        var control = new AlexaRating
        {
            RatingSlotPadding = new AbsoluteDimension(0,"dp"),
            RatingSlotMode = RatingSlotMode.Multiple,
            RatingNumber = 3.5,
            RatingText = "509 ratings",
            Spacing = "@spacingMedium"
        };
        Assert.True(Utility.CompareJson(control,"AlexaRating.json", _output));
    }

    [Fact]
    public void AlexaImageList()
    {
        var control = new AlexaImageList
        {
            ListItems = APLValue.To<IList<AlexaImageListItem>>("${imageListData.listItemsToShow}"),
            DefaultImageSource = "https://d2o906d8ln7ui1.cloudfront.net/images/BT7_Background.png",
            ImageBlurredBackground = true,
            PrimaryAction = (new APLCommand[]
            {
                new SendEvent
                {
                    Arguments = new[]{(object)"ListItemSelected", "${ordinal}"}.ToList()
                }
            }.ToList())
        };
        Assert.True(Utility.CompareJson(control,"AlexaImageList.json", _output));
    }

    [Fact]
    public void AlexaLists()
    {
        var control = new AlexaLists
        {
            ListItems = APLValue.To<IList<AlexaListItem>>("${listData.listItemsToShow}"),
            ListImagePrimacy = true,
            DefaultImageSource = "https://d2o906d8ln7ui1.cloudfront.net/images/BT7_Background.png",
            ImageBlurredBackground = true
        };

        Assert.True(Utility.CompareJson(control, "AlexaLists.json", _output));
    }

    [Fact]
    public void AlexaPaginatedList()
    {
        Utility.AssertComponent<AlexaPaginatedList>("AlexaPaginatedList.json", _output);
    }

    [Fact]
    public void TickHandler()
    {
        Utility.AssertComponent<Container>("TickHandler.json", _output);
    }

    [Fact]
    public void ProgressBar()
    {
        Utility.AssertComponent<AlexaProgressBar>("AlexaProgressBar.json", _output);
    }

    [Fact]
    public void ProgressBarRadial()
    {
        Utility.AssertComponent<Container>("AlexaProgressBarRadial.json", _output);
    }

    [Fact]
    public void ProgressDots()
    {
        Utility.AssertComponent<AlexaProgressDots>("AlexaProgressDots.json", _output);
    }

    [Fact]
    public void Slider()
    {
        Utility.AssertComponent<AlexaSlider>("AlexaSlider.json", _output);
    }

    [Fact]
    public void SliderRadial()
    {
        Utility.AssertComponent<Container>("AlexaSliderRadial.json", _output);
    }

    [Fact]
    public void AlexaDetailRecipe()
    {
        Utility.AssertComponent<AlexaDetail>("AlexaDetailRecipe.json", _output);
    }

    [Fact]
    public void AlexaDetailTv()
    {
        Utility.AssertComponent<AlexaDetail>("AlexaDetailTv.json", _output);
    }

    [Fact]
    public void AlexaGridList()
    {
        Utility.AssertComponent<AlexaGridList>("AlexaGridList.json", _output);
    }

    [Fact]
    public void EditText()
    {
        Utility.AssertComponent<EditText>("EditText.json", _output);
    }

    [Fact]
    public void SwipeToAction()
    {
        Utility.AssertComponent<AlexaSwipeToAction>("AlexaSwipeToAction.json", _output);
    }

    [Fact]
    public void AlexaRadioButton()
    {
        Utility.AssertComponent<AlexaRadioButton>("AlexaRadioButton.json", _output);
    }

    [Fact]
    public void AlexaCheckbox()
    {
        Utility.AssertComponent<AlexaCheckbox>("AlexaCheckbox.json", _output);
    }

    [Fact]
    public void AlexaSwitch()
    {
        Utility.AssertComponent<AlexaSwitch>("AlexaSwitch.json", _output);
    }

    [Fact]
    public void GridSequence()
    {
        Utility.AssertComponent<GridSequence>("GridSequence.json", _output);
    }

    [Fact]
    public void Pager()
    {
        Utility.AssertComponent<Pager>("Pager.json", _output);
    }

    [Fact]
    public void AlexaIcon()
    {
        Utility.AssertComponent<AlexaIcon>("AlexaIcon.json");
    }

    [Fact]
    public void AlexaCard()
    {
        Utility.AssertComponent<AlexaCard>("AlexaCard.json");
    }

    [Fact]
    public void AlexaImageCaption()
    {
        Utility.AssertComponent<AlexaImageCaption>("AlexaImageCaption.json");
    }

    [Fact]
    public void AlexaPhoto()
    {
        Utility.AssertComponent<AlexaPhoto>("AlexaPhoto.json");
    }

    [Fact]
    public void AlexaTextWrapping()
    {
        Utility.AssertComponent<AlexaTextWrapping>("AlexaTextWrapping.json");
    }

    [Fact]
    public void Frame()
    {
        Utility.AssertComponent<Frame>("Frame.json");
    }

    // [Fact]
    // public void DictionaryBindingTest()
    // {
    //     var rawContainer = new Container
    //     {
    //         Data = new[]{new Dictionary<string, object> { { "test", "thing" } }},
    //     };
    //     var dataBoundContainer = new Container
    //     {
    //         Data = APLValue.To<IList<object>>("$data.random.stuff")
    //     };
    //
    //     var rawJson = JsonConvert.SerializeObject(rawContainer);
    //     var boundJson = JsonConvert.SerializeObject(dataBoundContainer);
    //
    //     var newRaw = JsonConvert.DeserializeObject<APLComponent>(rawJson);
    //     var newBound = JsonConvert.DeserializeObject<APLComponent>(boundJson);
    //
    //     var newRawContainer = Assert.IsType<Container>(newRaw);
    //     var newBoundContainer = Assert.IsType<Container>(newBound);
    //
    //     Assert.Single((JObject)newRawContainer.Data.Value.First());
    //     Assert.Equal("$data.random.stuff", newBoundContainer.Data.Expression);
    // }

    [Fact]
    public void DictionaryBindingTest()
    {
        var options = AlexaJsonOptions.DefaultOptions;

        var rawContainer = new Container
        {
            Data = new[] { new Dictionary<string, object> { { "test", "thing" } } }
        };
        var dataBoundContainer = new Container
        {
            Data = APLValue.To<IList<object>>("$data.random.stuff")
        };

        var rawJson = JsonSerializer.Serialize(rawContainer, options);
        var boundJson = JsonSerializer.Serialize(dataBoundContainer, options);

        var newRaw = JsonSerializer.Deserialize<APLComponent>(rawJson, options);
        var newBound = JsonSerializer.Deserialize<APLComponent>(boundJson, options);

        var newRawContainer = Assert.IsType<Container>(newRaw);
        var newBoundContainer = Assert.IsType<Container>(newBound);

        var dataValue = Assert.IsAssignableFrom<IList<object>>(newRawContainer.Data.Value);
        var dict = Assert.IsType<Dictionary<string, object>>(dataValue.First());

        Assert.Single(dict);
        Assert.Equal("thing", dict["test"]);

        Assert.Equal("$data.random.stuff", newBoundContainer.Data.Expression);
    }

    private APLComponent GenerateComponent(string componentType)
    {
        var json = $$"""
                     {
                         "type": "{{componentType}}",
                         "numbered": true
                     }
                     """;
        return JsonSerializer.Deserialize<APLComponent>(json, AlexaJsonOptions.DefaultOptions)!;
    }
}