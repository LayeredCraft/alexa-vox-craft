using AlexaVoxCraft.Model.Apl.Filters;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class ImageFilterListConverter : SingleOrListConverter<IImageFilter>
{
    public ImageFilterListConverter(bool alwaysOutputArray) : base(alwaysOutputArray) { }
}