using AlexaVoxCraft.MediatR;
using AlexaVoxCraft.MediatR.Pipeline;
using Sample.Generated.Function.Resources;

namespace Sample.Generated.Function.Interceptors.Request;

public sealed class LocalizationRequestInterceptor : IRequestInterceptor
{
    public Task Process(IHandlerInput input, CancellationToken cancellationToken = default)
    {
        LanguageStrings.Culture = new System.Globalization.CultureInfo(input.RequestEnvelope.Request.Locale);
        return Task.CompletedTask;

    }
}