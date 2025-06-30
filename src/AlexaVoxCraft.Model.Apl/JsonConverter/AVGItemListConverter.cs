using AlexaVoxCraft.Model.Apl.VectorGraphics;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class AVGItemListConverter : SingleOrListConverter<IAVGItem>
{
    public AVGItemListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }
}