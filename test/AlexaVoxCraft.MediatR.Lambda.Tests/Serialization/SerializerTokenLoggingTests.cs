using System.Text;
using System.Text.Json;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Serialization;
using Compono;
using Compono.Logging;
using Compono.XunitV3;
using Microsoft.Extensions.Logging;
using LambdaSerializer = AlexaVoxCraft.Lambda.Serialization.AlexaLambdaSerializer;
using MediatRLambdaSerializer = AlexaVoxCraft.MediatR.Lambda.Serialization.AlexaLambdaSerializer;

namespace AlexaVoxCraft.MediatR.Lambda.Tests.Serialization;

/// <summary>
/// Regression tests: Alexa authentication tokens must never reach diagnostic output, even with
/// Debug logging enabled and a structured provider that destructures logged objects.
/// </summary>
[Collection("DiagnosticsConfig Tests")]
public class SerializerTokenLoggingTests : TestBase
{
    private const string ApiAccessTokenSentinel = "API-ACCESS-TOKEN-SENTINEL";
    private const string UserAccessTokenSentinel = "USER-ACCESS-TOKEN-SENTINEL";
    private const string PersonAccessTokenSentinel = "PERSON-ACCESS-TOKEN-SENTINEL";
    private const string ConsentTokenSentinel = "CONSENT-TOKEN-SENTINEL";

    private static readonly string[] Sentinels =
    [
        ApiAccessTokenSentinel, UserAccessTokenSentinel, PersonAccessTokenSentinel, ConsentTokenSentinel
    ];

    private const string RequestJson = $$"""
        {
          "version": "1.0",
          "session": {
            "new": true,
            "sessionId": "amzn1.echo-api.session.test",
            "application": { "applicationId": "amzn1.ask.skill.test" },
            "user": {
              "userId": "amzn1.ask.account.test",
              "accessToken": "{{UserAccessTokenSentinel}}",
              "permissions": { "consentToken": "{{ConsentTokenSentinel}}" }
            }
          },
          "context": {
            "System": {
              "apiAccessToken": "{{ApiAccessTokenSentinel}}",
              "apiEndpoint": "https://api.amazonalexa.com",
              "application": { "applicationId": "amzn1.ask.skill.test" },
              "user": {
                "userId": "amzn1.ask.account.test",
                "accessToken": "{{UserAccessTokenSentinel}}",
                "permissions": { "consentToken": "{{ConsentTokenSentinel}}" }
              },
              "person": {
                "personId": "amzn1.ask.person.test",
                "accessToken": "{{PersonAccessTokenSentinel}}"
              }
            }
          },
          "request": {
            "type": "LaunchRequest",
            "requestId": "amzn1.echo-api.request.test",
            "timestamp": "2026-11-24T00:00:00Z",
            "locale": "en-US"
          }
        }
        """;

    [Theory]
    [Compose<SerializerLoggingProfile>]
    public void LambdaSerializer_Deserialize_WithDebugLogging_DoesNotLogTokens(ILogger<LambdaSerializer> logger)
    {
        var serializer = new LambdaSerializer(logger, null);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(RequestJson));

        var request = serializer.Deserialize<SkillRequest>(stream);

        request!.Context.System.ApiAccessToken.Should().Be(ApiAccessTokenSentinel);
        AssertNoTokens(logger.GetCapturedEntries());
    }

    [Theory]
    [Compose<SerializerLoggingProfile>]
    public void MediatRLambdaSerializer_Deserialize_WithDebugLogging_DoesNotLogTokens(
        ILogger<MediatRLambdaSerializer> logger)
    {
        var serializer = new MediatRLambdaSerializer(logger, null);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(RequestJson));

        var request = serializer.Deserialize<SkillRequest>(stream);

        request!.Context.System.ApiAccessToken.Should().Be(ApiAccessTokenSentinel);
        AssertNoTokens(logger.GetCapturedEntries());
    }

    // Renders every captured message, structured property and scope the way a destructuring
    // structured provider (for example Serilog with {@Value}) would, then searches for tokens.
    private static void AssertNoTokens(IReadOnlyList<CapturedLogEntry> entries)
    {
        entries.Should().Contain(e => e.LogLevel == LogLevel.Debug, "Debug logging must be enabled for this test");

        var rendered = new StringBuilder();
        foreach (var entry in entries)
        {
            rendered.AppendLine(entry.Message);
            foreach (var property in entry.Properties ?? [])
            {
                rendered.AppendLine(Render(property.Value));
            }

            foreach (var scope in entry.Scopes)
            {
                rendered.AppendLine(Render(scope));
            }
        }

        var output = rendered.ToString();
        foreach (var sentinel in Sentinels)
        {
            output.Should().NotContain(sentinel);
        }
    }

    private static string Render(object? value) => value switch
    {
        null => string.Empty,
        string text => text,
        _ => JsonSerializer.Serialize(value, value.GetType(), AlexaJsonOptions.DefaultOptions)
    };

    public sealed class SerializerLoggingProfile : ICompositionProfile
    {
        public void Configure(CompositionBuilder builder) =>
            builder.UseLogging(options => options.MinimumLevel = LogLevel.Debug);
    }
}
