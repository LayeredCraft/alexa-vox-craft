using System.Reflection;
using AlexaVoxCraft.Model.Response;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.RequestSpecifications;

public class SkillResponseSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request) => request is ParameterInfo parameter &&
                                                 typeof(SkillResponse).IsAssignableFrom(parameter.ParameterType) ||
                                                 request is Type type && type == typeof(SkillResponse);
}