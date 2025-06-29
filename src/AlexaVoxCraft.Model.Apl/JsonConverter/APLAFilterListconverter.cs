namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLAFilterListConverter : SingleOrListConverter<APLAFilter>
{
    public APLAFilterListConverter(bool alwaysOutputArray = false) : base(alwaysOutputArray) { }
}