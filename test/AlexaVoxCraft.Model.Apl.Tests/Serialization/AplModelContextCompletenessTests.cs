using System.Reflection;
using AlexaVoxCraft.Model.Apl.Serialization;

namespace AlexaVoxCraft.Model.Apl.Tests.Serialization;

/// <summary>
/// Walks every BasePolymorphicConverter&lt;T&gt; subclass in AlexaVoxCraft.Model.Apl (and the shared
/// AlexaVoxCraft.Model assembly, since its converters may reference APL-owned types too) via test-only
/// reflection and asserts AplModelContext has metadata for every concrete type any of them can dispatch
/// to. This directly catches "forgot to add a type to [JsonSerializable]" as a test failure instead of a
/// first-use production surprise under Native AOT.
/// </summary>
public class AplModelContextCompletenessTests
{
    [Fact]
    public void AplModelContext_HasMetadata_ForEveryPolymorphicDispatchTarget()
    {
        var converterBaseType = typeof(APLComponent).Assembly.GetTypes()
            .Concat(typeof(AlexaVoxCraft.Model.Response.SkillResponse).Assembly.GetTypes())
            .FirstOrDefault(t => t.Name == "BasePolymorphicConverter`1");

        converterBaseType.Should().NotBeNull("BasePolymorphicConverter<T> must exist in one of the two assemblies");

        var converterTypes = typeof(APLComponent).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && IsSubclassOfRawGeneric(converterBaseType!, t));

        var missing = new List<string>();

        foreach (var converterType in converterTypes)
        {
            var derivedTypesProp = converterType.GetProperty("DerivedTypes", BindingFlags.Instance | BindingFlags.NonPublic);
            if (derivedTypesProp is null)
            {
                continue;
            }

            object? instance;
            try
            {
                instance = Activator.CreateInstance(converterType);
            }
            catch (MissingMethodException)
            {
                continue;
            }
            catch (TargetInvocationException)
            {
                continue;
            }

            if (derivedTypesProp.GetValue(instance) is not System.Collections.IDictionary dict)
            {
                continue;
            }

            foreach (var value in dict.Values)
            {
                if (value is not Type targetType)
                {
                    continue;
                }

                if (targetType.Assembly != typeof(APLComponent).Assembly)
                {
                    // Owned by another package's context (e.g. AlexaVoxCraft.Model) - out of scope here.
                    continue;
                }

                var typeInfo = AplModelContext.Default.GetTypeInfo(targetType);
                if (typeInfo is null)
                {
                    missing.Add($"{converterType.Name} -> {targetType.FullName}");
                }
            }
        }

        missing.Should().BeEmpty();
    }

    private static bool IsSubclassOfRawGeneric(Type genericBase, Type toCheck)
    {
        var current = toCheck.BaseType;
        while (current is not null)
        {
            var currentGeneric = current.IsGenericType ? current.GetGenericTypeDefinition() : current;
            if (currentGeneric == genericBase)
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }
}
