namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLComponentListConverter : SingleOrListConverter<APLComponent>
{
    public APLComponentListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }
}