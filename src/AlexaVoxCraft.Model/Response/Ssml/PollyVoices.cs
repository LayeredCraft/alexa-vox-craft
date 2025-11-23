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
public static class PollyVoices
{
    /// <summary>
    /// Contains voice IDs that have a Generative engine variant in Amazon Polly.
    /// <para>
    /// All of these voices are also available as conversational NTTS voices.
    /// </para>
    /// </summary>
    public static class Generative
    {
        // English

        /// <summary>English (Australian), female.</summary>
        public const string Olivia = "Olivia";           // en-AU

        /// <summary>English (Indian), female.</summary>
        public const string Kajal = "Kajal";             // en-IN

        /// <summary>English (Ireland), female.</summary>
        public const string Niamh = "Niamh";             // en-IE

        /// <summary>English (South African), female.</summary>
        public const string Ayanda = "Ayanda";           // en-ZA

        /// <summary>English (UK), female.</summary>
        public const string Amy = "Amy";                 // en-GB

        /// <summary>English (US), female.</summary>
        public const string Danielle = "Danielle";       // en-US

        /// <summary>English (US), female.</summary>
        public const string Joanna = "Joanna";           // en-US

        /// <summary>English (US), male.</summary>
        public const string Matthew = "Matthew";         // en-US

        /// <summary>English (US), female.</summary>
        public const string Ruth = "Ruth";               // en-US

        /// <summary>English (US), female.</summary>
        public const string Salli = "Salli";             // en-US

        /// <summary>English (US), male.</summary>
        public const string Stephen = "Stephen";         // en-US

        // Dutch

        /// <summary>Dutch (Belgian), female.</summary>
        public const string Lisa = "Lisa";               // nl-BE

        /// <summary>Dutch (Netherlands), female.</summary>
        public const string Laura = "Laura";             // nl-NL

        // French

        /// <summary>French (Belgian), female.</summary>
        public const string Isabelle = "Isabelle";       // fr-BE

        /// <summary>French (Canadian), female.</summary>
        public const string Gabrielle = "Gabrielle";     // fr-CA

        /// <summary>French (Canadian), male.</summary>
        public const string Liam = "Liam";               // fr-CA

        /// <summary>French (France), female (Céline/Celine).</summary>
        public const string Celine = "Celine";           // fr-FR

        /// <summary>French (France), female.</summary>
        public const string Lea = "Lea";                 // fr-FR

        /// <summary>French (France), male.</summary>
        public const string Remi = "Remi";               // fr-FR

        // German

        /// <summary>German (Austria), female.</summary>
        public const string Hannah = "Hannah";           // de-AT

        /// <summary>German (Germany), male.</summary>
        public const string Daniel = "Daniel";           // de-DE

        /// <summary>German (Germany), female.</summary>
        public const string Vicki = "Vicki";             // de-DE

        // Italian

        /// <summary>Italian (Italy), female.</summary>
        public const string Bianca = "Bianca";           // it-IT

        // Korean

        /// <summary>Korean (Korea), female.</summary>
        public const string Seoyeon = "Seoyeon";         // ko-KR

        // Polish

        /// <summary>Polish (Poland), female.</summary>
        public const string Ewa = "Ewa";                 // pl-PL

        /// <summary>Polish (Poland), female.</summary>
        public const string Ola = "Ola";                 // pl-PL

        // Portuguese

        /// <summary>Portuguese (Brazilian), female.</summary>
        public const string Camila = "Camila";           // pt-BR

        // Spanish

        /// <summary>Spanish (Mexican), male (Andrés/Andres).</summary>
        public const string Andres = "Andres";           // es-MX

        /// <summary>Spanish (Mexican), female (Mía/Mia).</summary>
        public const string Mia = "Mia";                 // es-MX

        /// <summary>Spanish (Spain), female.</summary>
        public const string Lucia = "Lucia";             // es-ES

        /// <summary>Spanish (Spain), male.</summary>
        public const string Sergio = "Sergio";           // es-ES

        /// <summary>Spanish (US), female.</summary>
        public const string Lupe = "Lupe";               // es-US

        /// <summary>Spanish (US), male.</summary>
        public const string Pedro = "Pedro";             // es-US
    }

    /// <summary>
    /// Contains voice IDs that have a Long-form engine variant in Amazon Polly.
    /// <para>
    /// Long-form voices are optimized for longer content such as articles, training
    /// material, or marketing videos, and are also based on generative TTS technology.
    /// </para>
    /// </summary>
    public static class LongForm
    {
        // English (US)

        /// <summary>English (US), female long-form voice.</summary>
        public const string Danielle = "Danielle";       // en-US

        /// <summary>English (US), male long-form voice.</summary>
        public const string Gregory = "Gregory";         // en-US

        /// <summary>English (US), female long-form voice.</summary>
        public const string Ruth = "Ruth";               // en-US

        /// <summary>English (US), male long-form voice.</summary>
        public const string Patrick = "Patrick";         // en-US

        // Spanish (Spain)

        /// <summary>Spanish (Spain), female long-form voice.</summary>
        public const string Alba = "Alba";               // es-ES

        /// <summary>Spanish (Spain), male long-form voice (Raúl/Raul).</summary>
        public const string Raul = "Raul";               // es-ES
    }
}
