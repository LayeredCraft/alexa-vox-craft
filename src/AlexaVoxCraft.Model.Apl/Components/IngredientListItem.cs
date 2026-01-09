using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.Apl.Components;

public class IngredientListItem
{
    [JsonPropertyName("ingredientsContentText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValue<string>? IngredientsContentText { get; set; }

    [JsonPropertyName("ingredientsPrimaryAction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APLValueCollection<APLCommand>? IngredientsPrimaryAction { get; set; }
}