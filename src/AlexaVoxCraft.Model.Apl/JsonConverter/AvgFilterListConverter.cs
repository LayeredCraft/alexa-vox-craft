using AlexaVoxCraft.Model.Apl.VectorGraphics.Filters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class AvgFilterListConverter : SingleOrListConverter<IAVGFilter>
{
    public AvgFilterListConverter(bool alwaysOutputArray) : base(alwaysOutputArray) { }
}