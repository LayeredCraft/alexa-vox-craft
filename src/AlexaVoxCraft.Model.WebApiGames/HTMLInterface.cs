using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.Model.WebApiGames;

/// <summary>
/// Helpers for the Alexa Web API for Games HTML interface capability.
/// </summary>
public static class HTMLInterface
{
    /// <summary>The supported-interfaces key for the Web API for Games HTML interface.</summary>
    public const string InterfaceName = "Alexa.Presentation.HTML";

    extension(SkillRequest request)
    {
        /// <summary>
        /// Determines whether the request's device advertises the HTML interface. Null-safe: returns
        /// <see langword="false"/> when the request, its context, system, device or supported-interfaces
        /// map is null. Only the presence of the key is inspected, never its value shape.
        /// </summary>
        public bool HTMLSupported() => throw new NotImplementedException();
    }
}
