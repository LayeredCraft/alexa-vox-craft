using Compono;
using Compono.Http;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.TestKit;

file static class HttpTestHarnessRowInvokerRegistration
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Register() =>
        RowInvokerRegistry.Register(
            typeof(HttpTestHarness),
            static (row, in descriptor) => row.Resolve<HttpTestHarness>(descriptor),
            static (row, in descriptor) => row.ResolveShared<HttpTestHarness>(descriptor),
            static (row, in descriptor, value) => row.ShareExplicit(descriptor, (HttpTestHarness)value!));
}

/// <summary>
/// Project-local Compono row parameter for HTTP client tests. Rider can execute xUnit discovery in
/// a way that misses row-binding registrations for parameter types owned by referenced assemblies;
/// wrapping <see cref="TestHttpHandler"/> keeps the composed theory parameter in this test assembly
/// while preserving the Compono.Http request/response API the tests exercise.
/// </summary>
public sealed class HttpTestHarness : IDisposable
{
    private readonly TestHttpHandler _handler = new();

    public IReadOnlyList<HttpRequestMessage> Requests => _handler.Requests;

    public HttpResponseRegistrationBuilder OnGet(string path) => _handler.OnGet(path);

    public HttpResponseRegistrationBuilder OnGet(Match<string> path) => _handler.OnGet(path);

    public HttpClient CreateClient(Uri? baseAddress = null) => _handler.CreateClient(baseAddress);

    public void Dispose() => _handler.Dispose();
}
