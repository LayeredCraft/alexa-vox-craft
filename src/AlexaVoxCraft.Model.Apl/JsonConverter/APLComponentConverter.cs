using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using AlexaVoxCraft.Model.Apl.Components;
using AlexaVoxCraft.Model.Response.Converters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLComponentConverter :BasePolymorphicConverter<APLComponent>
{
    private static readonly ConcurrentDictionary<string, Type> AplComponentLookup = new()
    {
        [nameof(Container)] = typeof(Container),
        [nameof(Image)] = typeof(Image),
        [nameof(Text)] = typeof(Text),
        [nameof(ScrollView)] = typeof(ScrollView),
        [nameof(Video)] = typeof(Video),
        [nameof(TimeText)] = typeof(TimeText),
        [nameof(TouchWrapper)] = typeof(TouchWrapper),
        [nameof(AlexaIconButton)] = typeof(AlexaIconButton),
        [nameof(AlexaImageListItem)] = typeof(AlexaImageListItem),
        [nameof(AlexaRating)] = typeof(AlexaRating),
        [nameof(AlexaImageList)] = typeof(AlexaImageList),
        [nameof(AlexaLists)] = typeof(AlexaLists),
        [nameof(AlexaPaginatedList)] = typeof(AlexaPaginatedList),
        [nameof(Frame)] = typeof(Frame),
        [nameof(AlexaProgressBar)] = typeof(AlexaProgressBar),
        [nameof(AlexaProgressBarRadial)] = typeof(AlexaProgressBarRadial),
        [nameof(AlexaProgressDots)] = typeof(AlexaProgressDots),
        [nameof(AlexaSlider)] = typeof(AlexaSlider),
        [nameof(AlexaSliderRadial)] = typeof(AlexaSliderRadial),
        [nameof(AlexaDetail)] = typeof(AlexaDetail),
        [nameof(AlexaGridList)] = typeof(AlexaGridList),
        [nameof(EditText)] = typeof(EditText),
        [nameof(AlexaSwipeToAction)] = typeof(AlexaSwipeToAction),
        [nameof(AlexaRadioButton)] = typeof(AlexaRadioButton),
        [nameof(AlexaCheckbox)] = typeof(AlexaCheckbox),
        [nameof(AlexaSwitch)] = typeof(AlexaSwitch),
        [nameof(GridSequence)] = typeof(GridSequence),
        [nameof(Pager)] = typeof(Pager),
        [nameof(AlexaIcon)] = typeof(AlexaIcon),
        [nameof(AlexaCard)] = typeof(AlexaCard),
        [nameof(AlexaImageCaption)] = typeof(AlexaImageCaption),
        [nameof(AlexaPhoto)] = typeof(AlexaPhoto),
        [nameof(AlexaTextWrapping)] = typeof(AlexaTextWrapping),
        [nameof(AlexaFooter)] = typeof(AlexaFooter),
        [nameof(AlexaHeader)] = typeof(AlexaHeader),
        [nameof(AlexaImage)] = typeof(AlexaImage),
        [nameof(AlexaBackground)] = typeof(AlexaBackground),
        [nameof(AlexaButton)] = typeof(AlexaButton),
        [nameof(AlexaDivider)] = typeof(AlexaDivider),
        [nameof(AlexaHeadline)] = typeof(AlexaHeadline),
        [nameof(AlexaOrdinal)] = typeof(AlexaOrdinal),
        [nameof(AlexaPageCounter)] = typeof(AlexaPageCounter),
        [nameof(AlexaTextList)] = typeof(AlexaTextList),
        [nameof(AlexaTextListItem)] = typeof(AlexaTextListItem),
        [nameof(AlexaTransportControls)] = typeof(AlexaTransportControls),
        [nameof(Sequence)] = typeof(Sequence),
        [nameof(Spacer)] = typeof(Spacer),
        [nameof(VectorGraphic)] = typeof(VectorGraphic),
    };
    protected override IDictionary<string, Type> DerivedTypes => AplComponentLookup;
    protected override JsonElement TransformJson(JsonElement original)
    {
        var obj = original.Deserialize<JsonObject>() ?? new JsonObject();

        string? type = obj["type"]?.GetValue<string>();
        if (string.IsNullOrEmpty(type))
        {
            return original;
        }

        // Universal field renames
        Move(obj, "gesture", "gestures");
        Move(obj, "entity", "entities");

        // Type-specific remapping
        switch (type)
        {
            case "GridSequence":
                Move(obj, "childHeight", "childHeights");
                Move(obj, "childWidth", "childWidths");

                if (obj.ContainsKey("item"))
                    Move(obj, "item", "items");
                break;

            case "Frame":
            case "TouchWrapper":
                if (obj.ContainsKey("items"))
                    Move(obj, "items", "item");
                break;

            case "Container":
            case "Pager":
                if (obj.ContainsKey("item"))
                    Move(obj, "item", "items");
                break;
        }

        return JsonSerializer.Deserialize<JsonElement>(obj.ToJsonString());
    }

    private static void Move(JsonObject json, string from, string to)
    {
        if (json.TryGetPropertyValue(from, out var value))
        {
            json[to] = value!.DeepClone(); // 👈 Clone before reassigning
            json.Remove(from);
        }
    }

    protected override Type? DefaultType => typeof(CustomComponent);
}