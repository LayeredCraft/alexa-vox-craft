using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response.Converters;
using AlexaVoxCraft.Model.Serialization;
using AlexaVoxCraft.Model.WebApiGames.RequestHandlers;
using AlexaVoxCraft.Model.WebApiGames.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// Entry point for registering all Web API for Games support with the AlexaVoxCraft serialization infrastructure.
/// </summary>
public static class WebApiGamesSupport
{
    // RegisterPackageTypeInfoResolver appends without deduplicating, so the package registration is
    // guarded once; the other two seams (RequestConverter/DirectiveConverter) already dedupe internally.
    private static int _registered;

    /// <summary>
    /// Registers the Web API for Games request and directive types. Call this once at application startup
    /// before processing skill requests. Idempotent and order-independent with the other package registrations.
    /// </summary>
    public static void Add()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        AlexaJsonOptions.RegisterPackageTypeInfoResolver(WebApiGamesModelContext.Default);
        DirectiveConverter.RegisterDirectiveDerivedType<StartDirective>(StartDirective.DirectiveType);
        DirectiveConverter.RegisterDirectiveDerivedType<HandleMessageDirective>(HandleMessageDirective.DirectiveType);
        RequestConverter.RegisterRequestTypeResolver<MessageRequestTypeResolver>();
    }
}
