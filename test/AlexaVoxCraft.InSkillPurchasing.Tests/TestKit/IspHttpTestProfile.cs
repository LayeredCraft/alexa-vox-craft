using AlexaVoxCraft.InSkillPurchasing.Clients;
using Compono;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.TestKit;

/// <summary>
/// Shared composition profile for In-Skill Purchasing HTTP client tests. Mirrors the SMAPI HTTP
/// profile: tests configure a shared <see cref="HttpTestHarness"/>, while Compono composes the
/// client under test with an <see cref="HttpClient"/> created from that same handler.
/// </summary>
public sealed class IspHttpTestProfile : ICompositionProfile
{
    public void Configure(CompositionBuilder builder)
    {
        // HttpClient has multiple public constructors; select one so generator discovery succeeds.
        // The registration below still wins at runtime and supplies the real client instance.
        builder.For<HttpClient>().UseConstructor<HttpMessageHandler, bool>();

        builder
            .Register<HttpClient>(context =>
                context.Resolve<HttpTestHarness>().CreateClient(new Uri("https://api.amazonalexa.com/")))
            .Register<ILogger<InSkillPurchasingClient>>(() => NullLogger<InSkillPurchasingClient>.Instance);
    }
}
