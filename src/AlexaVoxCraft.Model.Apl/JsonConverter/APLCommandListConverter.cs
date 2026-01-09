namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLCommandListConverter : APLValueCollectionConverter<APLCommand>
{
    public APLCommandListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }

}