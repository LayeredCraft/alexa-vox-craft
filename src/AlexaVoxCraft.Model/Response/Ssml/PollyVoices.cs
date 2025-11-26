namespace AlexaVoxCraft.Model.Response.Ssml;

/// <summary>
/// Provides constants for Amazon Polly voice IDs that currently have
/// <c>Generative</c> and/or <c>Long-form</c> variants, as documented in
/// the Amazon Polly Developer Guide.
/// <para>
/// These values are intended to be used anywhere a Polly voice ID is required,
/// such as:
/// <list type="bullet">
///   <item>
///     <description>The <c>name</c> attribute on the SSML <c>&lt;voice&gt;</c> element.</description>
///   </item>
///   <item>
///     <description>The <c>VoiceId</c> parameter to Polly APIs (for example, <c>SynthesizeSpeech</c>).</description>
///   </item>
/// </list>
/// </para>
/// <para>
/// This list is a snapshot based on the AWS documentation at the time of writing,
/// and is not guaranteed to remain exhaustive as new voices are added.
/// See the official Amazon Polly documentation for the most up-to-date list.
/// </para>
/// </summary>
/// <remarks>
/// Voice IDs are provided in their ASCII form (for example, <c>Celine</c> instead of <c>Céline</c>,
/// <c>Andres</c> instead of <c>Andrés</c>) to match the IDs returned by the Polly
/// <c>DescribeVoices</c> API and the "Name/ID" column in the "Available voices" table.
/// </remarks>
[Obsolete("Polly generative/long-form voice constants are not guaranteed to be supported in Alexa. Prefer AlexaSupportedVoices for use in Alexa skills.")]
public static class PollyVoices
{
    /// <summary>
    /// Contains voice IDs that have a Generative engine variant in Amazon Polly.
    /// <para>
    /// All of these voices are also available as conversational NTTS voices.
    /// </para>
    /// </summary>
    [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
    public static class Generative
    {
        // English

        /// <summary>English (Australian), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Olivia = "Olivia";           // en-AU

        /// <summary>English (Indian), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Kajal = "Kajal";             // en-IN

        /// <summary>English (Ireland), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Niamh = "Niamh";             // en-IE

        /// <summary>English (South African), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Ayanda = "Ayanda";           // en-ZA

        /// <summary>English (UK), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.EnglishGB.Amy instead.")]
        public const string Amy = "Amy";                 // en-GB

        /// <summary>English (US), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Danielle = "Danielle";       // en-US

        /// <summary>English (US), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.EnglishUS.Joanna instead.")]
        public const string Joanna = "Joanna";           // en-US

        /// <summary>English (US), male.</summary>
        [Obsolete("Use AlexaSupportedVoices.EnglishUS.Matthew instead.")]
        public const string Matthew = "Matthew";         // en-US

        /// <summary>English (US), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Ruth = "Ruth";               // en-US

        /// <summary>English (US), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.EnglishUS.Salli instead.")]
        public const string Salli = "Salli";             // en-US

        /// <summary>English (US), male.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Stephen = "Stephen";         // en-US

        // Dutch

        /// <summary>Dutch (Belgian), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Lisa = "Lisa";               // nl-BE

        /// <summary>Dutch (Netherlands), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Laura = "Laura";             // nl-NL

        // French

        /// <summary>French (Belgian), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Isabelle = "Isabelle";       // fr-BE

        /// <summary>French (Canadian), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Gabrielle = "Gabrielle";     // fr-CA

        /// <summary>French (Canadian), male.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Liam = "Liam";               // fr-CA

        /// <summary>French (France), female (Céline/Celine).</summary>
        [Obsolete("Use AlexaSupportedVoices.FrenchFR.Celine instead.")]
        public const string Celine = "Celine";           // fr-FR

        /// <summary>French (France), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.FrenchFR.Lea instead.")]
        public const string Lea = "Lea";                 // fr-FR

        /// <summary>French (France), male.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Remi = "Remi";               // fr-FR

        // German

        /// <summary>German (Austria), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Hannah = "Hannah";           // de-AT

        /// <summary>German (Germany), male.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Daniel = "Daniel";           // de-DE

        /// <summary>German (Germany), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.GermanDE.Vicki instead.")]
        public const string Vicki = "Vicki";             // de-DE

        // Italian

        /// <summary>Italian (Italy), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.ItalianIT.Bianca instead.")]
        public const string Bianca = "Bianca";           // it-IT

        // Korean

        /// <summary>Korean (Korea), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Seoyeon = "Seoyeon";         // ko-KR

        // Polish

        /// <summary>Polish (Poland), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Ewa = "Ewa";                 // pl-PL

        /// <summary>Polish (Poland), female.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Ola = "Ola";                 // pl-PL

        // Portuguese

        /// <summary>Portuguese (Brazilian), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.PortugueseBR.Camila instead.")]
        public const string Camila = "Camila";           // pt-BR

        // Spanish

        /// <summary>Spanish (Mexican), male (Andrés/Andres).</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Andres = "Andres";           // es-MX

        /// <summary>Spanish (Mexican), female (Mía/Mia).</summary>
        [Obsolete("Use AlexaSupportedVoices.SpanishMX.Mia instead.")]
        public const string Mia = "Mia";                 // es-MX

        /// <summary>Spanish (Spain), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.SpanishES.Lucia instead.")]
        public const string Lucia = "Lucia";             // es-ES

        /// <summary>Spanish (Spain), male.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Sergio = "Sergio";           // es-ES

        /// <summary>Spanish (US), female.</summary>
        [Obsolete("Use AlexaSupportedVoices.SpanishUS.Lupe instead.")]
        public const string Lupe = "Lupe";               // es-US

        /// <summary>Spanish (US), male.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Pedro = "Pedro";             // es-US
    }

    /// <summary>
    /// Contains voice IDs that have a Long-form engine variant in Amazon Polly.
    /// <para>
    /// Long-form voices are optimized for longer content such as articles, training
    /// material, or marketing videos, and are also based on generative TTS technology.
    /// </para>
    /// </summary>
    [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
    public static class LongForm
    {
        // English (US)

        /// <summary>English (US), female long-form voice.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Danielle = "Danielle";       // en-US

        /// <summary>English (US), male long-form voice.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Gregory = "Gregory";         // en-US

        /// <summary>English (US), female long-form voice.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Ruth = "Ruth";               // en-US

        /// <summary>English (US), male long-form voice.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Patrick = "Patrick";         // en-US

        // Spanish (Spain)

        /// <summary>Spanish (Spain), female long-form voice.</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Alba = "Alba";               // es-ES

        /// <summary>Spanish (Spain), male long-form voice (Raúl/Raul).</summary>
        [Obsolete("Use AlexaSupportedVoices instead for Alexa-compatible voice constants.")]
        public const string Raul = "Raul";               // es-ES
    }
}