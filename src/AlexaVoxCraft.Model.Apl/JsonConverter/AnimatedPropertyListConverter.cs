using AlexaVoxCraft.Model.Apl.Commands;

namespace AlexaVoxCraft.Model.Apl.JsonConverter;

public class AnimatedPropertyListConverter : SingleOrListConverter<AnimatedProperty>
{
    public AnimatedPropertyListConverter(bool alwaysOutputArray) : base(alwaysOutputArray)
    {
    }
}