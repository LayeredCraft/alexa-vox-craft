using System.Reflection;
using AlexaVoxCraft.Model.Request;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.RequestSpecifications;

public class SkillRequestSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request) => request is ParameterInfo parameter &&
                                                 typeof(SkillRequest).IsAssignableFrom(parameter.ParameterType) ||
                                                 request is Type type && type == typeof(SkillRequest);
}