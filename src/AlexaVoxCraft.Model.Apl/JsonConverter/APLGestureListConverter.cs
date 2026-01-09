namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class APLGestureListConverter : APLValueCollectionConverter<APLGesture>
{
    public APLGestureListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }
}