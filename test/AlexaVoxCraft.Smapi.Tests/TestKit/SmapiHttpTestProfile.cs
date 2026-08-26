using AlexaVoxCraft.Smapi.Auth;
using AlexaVoxCraft.Smapi.Clients;
using Compono;
using Compono.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AlexaVoxCraft.Smapi.Tests.TestKit;

/// <summary>
/// PLAN-0051 (Compono ecosystem migration): the shared composition profile for every
/// AlexaVoxCraft.Smapi.Tests test that needs a real <see cref="HttpClient"/> backed by a
/// <see cref="HttpTestHarness"/> - replaces the old AutoFixture-based
/// SmapiClientAutoDataAttribute/ClientAutoDataAttribute/HttpClientSpecimenBuilder chain.
/// Applied via <c>[Compose&lt;SmapiHttpTestProfile&gt;]</c>; pair with a
/// <c>[Shared] HttpTestHarness handler</c> theory parameter so the test's own configuration and
/// the SUT's injected <see cref="HttpClient"/> share the exact same handler instance (ADR-0051).
/// </summary>
public sealed class SmapiHttpTestProfile : ICompositionProfile
{
    public void Configure(CompositionBuilder builder)
    {
        // ADR-0002 Amendment 3/ADR-0052 (Part B): HttpClient has 3 accessible constructors, so any
        // composed type reaching it structurally (e.g. AlexaInteractionModelClient's own
        // HttpClient constructor parameter) triggers CMP0001 at compile time regardless of the
        // Register<HttpClient> below - compile-time discovery can't see a runtime registration.
        // This selects HttpClient's (HttpMessageHandler, bool) constructor so discovery succeeds;
        // the registration below still supplies the actual runtime value (a registration always
        // outranks a generated plan), so this has no effect on what any test actually observes.
        builder.For<HttpClient>().UseConstructor<HttpMessageHandler, bool>();

        builder
            .Register<HttpClient>(context =>
                context.Resolve<HttpTestHarness>().CreateClient(new Uri("https://api.amazonalexa.com/")))
            // AlexaInteractionModelClient/AlexaSkillInvocationClient's own ILogger<T> constructor
            // dependencies - no test here asserts on logged content, so a plain NullLogger<T> is
            // the right fallback, matching the same pattern MediatRTestProfile already uses for
            // ILogger<SkillMediator>.
            .Register<ILogger<AlexaInteractionModelClient>>(() => NullLogger<AlexaInteractionModelClient>.Instance)
            .Register<ILogger<AlexaSkillInvocationClient>>(() => NullLogger<AlexaSkillInvocationClient>.Instance)
            // SmapiDeveloperAccessTokenProvider's IHttpClientFactory seam - a 3-line project-local
            // fake, not a Compono.Http capability (ADR-0051 "IHttpClientFactory" - no native
            // support in v1; IHttpClientFactory is an ordinary single-method interface). Reuses
            // the same HttpClient/HttpTestHarness registered above, so requests made through it
            // are visible on the same [Shared] handler's Requests/registrations.
            .Register<IHttpClientFactory>(context => new FakeHttpClientFactory(context.Resolve<HttpClient>()))
            // SmapiDeveloperAccessTokenOptions has [Required] members validated in its consumer's
            // constructor (ArgumentException.ThrowIfNullOrWhiteSpace). Built directly from
            // provider-resolved strings (context.Resolve<string>() - string is a built-in simple
            // type, resolved unconditionally with no discovery needed) rather than
            // context.Resolve<SmapiDeveloperAccessTokenOptions>() for the record itself - verified
            // empirically that the latter throws CompositionException ("No ... generated plan
            // could satisfy") because the record is never independently reachable as a discovery
            // root anywhere in this project (Compono's compile-time discovery only walks real
            // Create<T>()/composed-theory-parameter roots; a type reached only via a nested
            // context.Resolve<T>() call inside a registration factory isn't itself a root the
            // generator can see, unlike a provider-resolved primitive/BCL value type).
            .Register<IOptions<SmapiDeveloperAccessTokenOptions>>(context =>
                Options.Create(new SmapiDeveloperAccessTokenOptions
                {
                    ClientId = context.Resolve<string>(),
                    ClientSecret = context.Resolve<string>(),
                    RefreshToken = context.Resolve<string>(),
                }));
    }

    private sealed class FakeHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }
}
