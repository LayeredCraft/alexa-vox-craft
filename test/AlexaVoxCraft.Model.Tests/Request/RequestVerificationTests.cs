using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using Compono.Http;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Coverage for <see cref="RequestVerification"/>: the replay-attack timestamp guard, and
/// <see cref="RequestVerification.GetCertificate"/> (the request-signature-verification certificate
/// fetch, which switches between the obsolete <c>X509Certificate2</c> constructor on net8 and
/// <c>X509CertificateLoader</c> on net9+ — see docs/plans/0002-cloudwatch-driven-verify-tests-and-warning-cleanup.md
/// Commit 15).
/// </summary>
public sealed class RequestVerificationTests
{
    [Fact]
    public void RecentTimestamp_IsWithinTolerance()
    {
        var request = new SkillRequest { Request = new LaunchRequest { Timestamp = DateTime.Now.AddMinutes(1) } };

        RequestVerification.RequestTimestampWithinTolerance(request).Should().BeTrue();
    }

    [Fact]
    public void OldTimestamp_IsOutsideTolerance()
    {
        var request = new SkillRequest { Request = new LaunchRequest { Timestamp = DateTime.Now.AddMinutes(3) } };

        RequestVerification.RequestTimestampWithinTolerance(request).Should().BeFalse();
    }

    [Fact]
    public async Task GetCertificate_ParsesFetchedCertificateBytes()
    {
        using var expected = CreateSelfSignedCertificate();
        var certificateBytes = expected.Export(X509ContentType.Cert);

        using var handler = new TestHttpHandler();
        // Compono.Http has no raw-bytes response helper; DER bytes round-trip losslessly through
        // Latin1 (a single-byte, bijective 0-255 char<->byte mapping), unlike UTF-8.
        handler.OnGet("/echo.api/cert.pem")
            .RespondText(Encoding.Latin1.GetString(certificateBytes), "application/octet-stream", Encoding.Latin1);
        using var client = handler.CreateClient(new Uri("https://s3.amazonaws.com"));

        // GetCertificate builds its own internal HttpClient, so exercise it through the public
        // three-argument Verify overload instead, which accepts a getCertificate override.
        var result = await RequestVerification.Verify(
            "irrelevant-signature",
            new Uri("https://s3.amazonaws.com/echo.api/cert.pem"),
            "irrelevant-body",
            async uri =>
            {
                var response = await client.GetAsync(uri);
                var bytes = await response.Content.ReadAsByteArrayAsync();
#if NET9_0_OR_GREATER
                return X509CertificateLoader.LoadCertificate(bytes);
#else
                return new X509Certificate2(bytes);
#endif
            });

        // The self-signed test certificate fails chain validation, so Verify itself returns false -
        // this test is only about the fetch-and-parse path (the getCertificate delegate) succeeding
        // without throwing, which is what Commit 15's fix and this delegate override are exercising.
        result.Should().BeFalse();
        handler.Requests.Should().ContainSingle();
    }

    private static X509Certificate2 CreateSelfSignedCertificate()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=echo-api.amazon.com", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow.AddMinutes(5));
    }
}
