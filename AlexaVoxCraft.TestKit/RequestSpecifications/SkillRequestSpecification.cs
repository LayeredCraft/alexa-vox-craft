using System.Reflection;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.RequestSpecifications;

public class SkillRequestSpecification : IRequestSpecification
{
    private readonly static Type[] ValidTypes = [typeof(SkillRequest), typeof(Request)];
    
    public bool IsSatisfiedBy(object request)
    {
        return request switch
        {
            ParameterInfo parameter => ValidTypes.Any(validType => validType.IsAssignableFrom(parameter.ParameterType)),
            Type type => ValidTypes.Contains(type),
            _ => false
        };
    }
}