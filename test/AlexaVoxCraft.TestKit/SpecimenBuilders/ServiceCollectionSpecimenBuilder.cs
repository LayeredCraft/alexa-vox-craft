using AlexaVoxCraft.TestKit.RequestSpecifications;
using AutoFixture.Kernel;
using Microsoft.Extensions.DependencyInjection;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public class ServiceCollectionSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public ServiceCollectionSpecimenBuilder() : this(new ServiceCollectionSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        return new ServiceCollection();
    }
}