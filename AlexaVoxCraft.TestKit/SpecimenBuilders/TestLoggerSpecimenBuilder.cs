using System.Reflection;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;
using LayeredCraft.StructuredLogging.Testing;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public class TestLoggerSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public TestLoggerSpecimenBuilder() : this(new TestLoggerSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        // Check if it's a generic ILogger<T>
        if (request is Type type && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ILogger<>))
        {
            var genericArgument = type.GetGenericArguments()[0];
            var testLoggerType = typeof(TestLogger<>).MakeGenericType(genericArgument);
            var testLogger = Activator.CreateInstance(testLoggerType)!;
            
            // Set minimum log level to Debug to ensure debug logs are captured
            var minLevelProperty = testLoggerType.GetProperty("MinimumLogLevel");
            minLevelProperty?.SetValue(testLogger, LogLevel.Debug);
            
            return testLogger;
        }

        // Check if it's a parameter with generic ILogger<T>
        if (request is ParameterInfo parameter && 
            parameter.ParameterType.IsGenericType && 
            parameter.ParameterType.GetGenericTypeDefinition() == typeof(ILogger<>))
        {
            var genericArgument = parameter.ParameterType.GetGenericArguments()[0];
            var testLoggerType = typeof(TestLogger<>).MakeGenericType(genericArgument);
            var testLogger = Activator.CreateInstance(testLoggerType)!;
            
            // Set minimum log level to Debug to ensure debug logs are captured
            var minLevelProperty = testLoggerType.GetProperty("MinimumLogLevel");
            minLevelProperty?.SetValue(testLogger, LogLevel.Debug);
            
            return testLogger;
        }

        return new NoSpecimen();
    }
}