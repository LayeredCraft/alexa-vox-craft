using System.Reflection;
using AutoFixture.Kernel;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.TestKit.RequestSpecifications;

public class TestLoggerSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
    {
        return request switch
        {
            Type type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ILogger<>),
            ParameterInfo parameter => parameter.ParameterType.IsGenericType && 
                                     parameter.ParameterType.GetGenericTypeDefinition() == typeof(ILogger<>),
            _ => false
        };
    }
}