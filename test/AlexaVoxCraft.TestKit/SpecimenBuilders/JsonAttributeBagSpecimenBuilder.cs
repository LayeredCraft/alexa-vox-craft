using System.Text.Json;
using AlexaVoxCraft.MediatR.Attributes;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public class JsonAttributeBagSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(JsonAttributeBag))
            return new JsonAttributeBag(new Dictionary<string, JsonElement>());

        return new NoSpecimen();
    }
}