using System.Text.Json;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

public sealed class JsonElementSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(JsonElement))
            return JsonSerializer.SerializeToElement(Guid.NewGuid().ToString());

        return new NoSpecimen();
    }
}
