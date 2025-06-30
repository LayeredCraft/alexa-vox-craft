using AutoFixture;
using AutoFixture.Kernel;
using LayeredCraft.StructuredLogging.Testing;
using Microsoft.Extensions.Logging;

namespace AlexaVoxCraft.TestKit.Customizations;

public class TestLoggerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // Use Register instead of a specimen builder to work with freezing
        fixture.Register<ILogger<AlexaVoxCraft.MediatR.Pipeline.PerformanceLoggingBehavior>>(() => 
            new TestLogger<AlexaVoxCraft.MediatR.Pipeline.PerformanceLoggingBehavior> { MinimumLogLevel = LogLevel.Debug });
        
        // For other logger types, use a relay that respects freezing
        fixture.Customizations.Add(new TestLoggerRelay());
    }

    private class TestLoggerRelay : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;
            if (type == null)
                return new NoSpecimen();

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ILogger<>))
                return new NoSpecimen();

            // Skip PerformanceLoggingBehavior as it's handled by Register above
            var genericArgument = type.GetGenericArguments()[0];
            if (genericArgument == typeof(AlexaVoxCraft.MediatR.Pipeline.PerformanceLoggingBehavior))
                return new NoSpecimen();

            var testLoggerType = typeof(TestLogger<>).MakeGenericType(genericArgument);
            var testLogger = Activator.CreateInstance(testLoggerType)!;
            
            // Set minimum log level to Debug to ensure debug logs are captured
            var minLevelProperty = testLoggerType.GetProperty("MinimumLogLevel");
            minLevelProperty?.SetValue(testLogger, LogLevel.Debug);
            
            return testLogger;
        }
    }
}