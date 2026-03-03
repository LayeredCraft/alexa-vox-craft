using System.Text.Json;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

public class JsonElementSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(JsonElement))
            return JsonSerializer.SerializeToElement(Guid.NewGuid().ToString());

        return new NoSpecimen();
    }
}