using AlexaVoxCraft.Http.TestKit.SpecimenBuilders;

namespace AlexaVoxCraft.Http.TestKit.Attributes;

/// <summary>
/// Provides auto-generated test data with configured HttpClient and HttpMessageHandler for client testing.
/// </summary>
public class ClientAutoDataAttribute(Func<IFixture> factory) : AutoDataAttribute(factory)
{
    public static IFixture CreateBaseFixture(Action<IFixture>? customizeAction = null)
    {
        return BaseFixtureFactory.CreateFixture(fixture =>
        {
            fixture.Freeze<HttpMessageHandler>();
            fixture.Customizations.Add(new HttpClientSpecimenBuilder());
            customizeAction?.Invoke(fixture);
        });
    }
}

/// <summary>
/// Provides inline test data combined with auto-generated client test data.
/// </summary>
public sealed class InlineClientAutoDataAttribute(params object?[] values)
    : InlineAutoDataAttribute(ClientAutoDataAttribute.CreateBaseFixture, values);