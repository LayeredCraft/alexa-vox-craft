using System.Reflection;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.TestKit.RequestSpecifications;

public abstract class BaseTypedSpecification<T> : IRequestSpecification
{
    public virtual bool IsSatisfiedBy(object request) =>
        request is ParameterInfo parameter && parameter.ParameterType == typeof(T) ||
        request is Type type && type == typeof(T);
}