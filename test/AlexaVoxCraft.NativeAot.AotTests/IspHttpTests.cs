using AlexaVoxCraft.InSkillPurchasing.Clients;
using Compono.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AlexaVoxCraft.NativeAot.AotTests;

// Scenario 6 (original console app): ISP/Http. Now genuinely dogfoods Compono.Http's TestHttpHandler
// instead of a hand-rolled FakeHttpMessageHandler - the whole point of adding Compono.Http as a
// dependency here, now that this project has real test-infra dependencies at all.
public sealed class IspHttpTests
{
    [Fact]
    public async Task InSkillPurchasingClient_GetProductsAsync_UsesAotSafeDefaultResolver()
    {
        var handler = new TestHttpHandler();
        handler.OnGet("/v1/users/~current/skills/~current/inSkillProducts")
            .RespondText("""{"inSkillProducts":[],"isTruncated":false}""", "application/json");

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.amazonalexa.com/") };
        var ispClient = new InSkillPurchasingClient(httpClient, NullLogger<InSkillPurchasingClient>.Instance);

        var products = await ispClient.GetProductsAsync(cancellationToken: CancellationToken.None);

        Assert.NotNull(products);
    }
}
