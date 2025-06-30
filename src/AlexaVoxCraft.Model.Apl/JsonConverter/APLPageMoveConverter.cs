namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLPageMoveConverter : SingleOrListConverter<APLPageMoveHandler>
{
    public APLPageMoveConverter(bool alwaysOutputArray) : base(alwaysOutputArray) { }
}