using AlexaVoxCraft.Http;
using AlexaVoxCraft.MediatR;

namespace AlexaVoxCraft.InSkillPurchasing.Auth;

/// <summary>
/// Provides the Alexa API access token extracted from the current skill request context.
/// </summary>
public class AlexaRequestAccessTokenProvider : IAccessTokenProvider
{
    private readonly SkillRequestFactory _skillRequestFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlexaRequestAccessTokenProvider"/> class.
    /// </summary>
    /// <param name="skillRequestFactory">A factory that resolves the current skill request.</param>
    public AlexaRequestAccessTokenProvider(SkillRequestFactory skillRequestFactory)
    {
        _skillRequestFactory = skillRequestFactory ?? throw new ArgumentNullException(nameof(skillRequestFactory));
    }

    /// <inheritdoc />
    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var skillRequest = _skillRequestFactory();
        var token = skillRequest?.Context.System.ApiAccessToken;

        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException(
                "ApiAccessToken is missing from the Alexa request context.");

        return Task.FromResult(token);
    }
}