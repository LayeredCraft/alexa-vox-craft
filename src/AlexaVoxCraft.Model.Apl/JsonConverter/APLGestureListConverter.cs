namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLGestureListConverter : SingleOrListConverter<APLGesture>
{
    public APLGestureListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }
}