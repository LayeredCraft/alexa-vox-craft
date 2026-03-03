using System.Net.Http.Headers;
using AlexaVoxCraft.MediatR;

namespace AlexaVoxCraft.InSkillPurchasing;

/// <summary>
/// A delegating handler that sets the <c>Accept-Language</c> header on outgoing HTTP requests
/// based on the locale of the current Alexa skill request.
/// </summary>
public class LocaleHandler : DelegatingHandler
{
    private readonly SkillRequestFactory _skillRequestFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocaleHandler"/> class.
    /// </summary>
    /// <param name="skillRequestFactory">A factory that resolves the current skill request.</param>
    public LocaleHandler(SkillRequestFactory skillRequestFactory)
    {
        _skillRequestFactory = skillRequestFactory ?? throw new ArgumentNullException(nameof(skillRequestFactory));
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var skillRequest = _skillRequestFactory();
        var locale = skillRequest?.Request.Locale;
        if (string.IsNullOrWhiteSpace(locale))
            throw new InvalidOperationException(
                "Locale is missing from the Alexa request context.");

        request.Headers.AcceptLanguage.Clear();
        request.Headers.AcceptLanguage.Add(
            new StringWithQualityHeaderValue(locale));
        return await base.SendAsync(request, cancellationToken);
    }
}