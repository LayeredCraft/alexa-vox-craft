using System.Reflection;
using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public class ServiceProviderSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public ServiceProviderSpecimenBuilder() : this(new ServiceProviderSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var services = new ServiceCollection();
        
        // Add substituted logger
        var logger = (ILogger<SkillMediator>)context.Resolve(typeof(ILogger<SkillMediator>));
        services.AddSingleton(logger);
        
        // Add IHandlerInput
        var handlerInput = (IHandlerInput)context.Resolve(typeof(IHandlerInput));
        services.AddSingleton(handlerInput);
        
        // Build and return the actual service provider
        return services.BuildServiceProvider();
    }
}