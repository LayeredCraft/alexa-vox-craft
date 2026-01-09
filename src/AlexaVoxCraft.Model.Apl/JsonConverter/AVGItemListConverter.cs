using AlexaVoxCraft.Model.Apl.VectorGraphics;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class AVGItemListConverter : APLValueCollectionConverter<IAVGItem>
{
    public AVGItemListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }
}