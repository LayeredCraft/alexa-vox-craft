using System.Reflection;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Smapi.Models.Invocation;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.RequestSpecifications;

public sealed class SkillInvocationResponseSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
    {
        return request is Type type && type == typeof(SkillInvocationResponse<SkillResponse>) ||
               (request is ParameterInfo parameter && parameter.ParameterType == typeof(SkillInvocationResponse<SkillResponse>));
    }
}