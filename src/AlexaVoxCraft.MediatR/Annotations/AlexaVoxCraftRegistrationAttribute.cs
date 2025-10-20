namespace AlexaVoxCraft.MediatR.Annotations;

/// <summary>
/// Marks a partial class as the target for source-generated DI registration.
/// The generator will implement the partial AddAlexaSkillMediator method in this class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class AlexaVoxCraftRegistrationAttribute : Attribute
{
}