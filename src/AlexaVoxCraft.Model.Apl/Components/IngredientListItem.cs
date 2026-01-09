using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AlexaVoxCraft.Model.Apl.JsonConverter;
using AlexaVoxCraft.Model.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class IngredientListItem : IJsonSerializable<IngredientListItem>
{
    [JsonPropertyName("ingredientsContentText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? IngredientsContentText { get; set; }

    [JsonPropertyName("ingredientsPrimaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>>? IngredientsPrimaryAction { get; set; }

    public static void RegisterTypeInfo<T>() where T : IngredientListItem
    {
        AlexaJsonOptions.RegisterTypeModifier<T>(info =>
        {
            var ingredientsPrimaryActionProp =
                info.Properties.FirstOrDefault(p => p.Name == "ingredientsPrimaryAction");
            if (ingredientsPrimaryActionProp is not null)
            {
                ingredientsPrimaryActionProp.CustomConverter = new APLCommandListConverter(false);
            }
        });
    }
}