namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLCommandListConverter : SingleOrListConverter<APLCommand>
{
    public APLCommandListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }

}