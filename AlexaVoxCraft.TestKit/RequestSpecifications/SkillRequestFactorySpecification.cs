using System.Reflection;
using AlexaVoxCraft.MediatR;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.RequestSpecifications;

public class SkillRequestFactorySpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request) => request is ParameterInfo parameter &&
                                                 typeof(SkillRequestFactory).IsAssignableFrom(parameter.ParameterType) ||
                                                 request is Type type && type == typeof(SkillRequestFactory);
}