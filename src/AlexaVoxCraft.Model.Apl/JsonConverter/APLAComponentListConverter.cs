namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLAComponentListConverter : SingleOrListConverter<APLAComponent>
{
    public APLAComponentListConverter(bool alwaysOutputArray = false) : base(alwaysOutputArray) { }
}