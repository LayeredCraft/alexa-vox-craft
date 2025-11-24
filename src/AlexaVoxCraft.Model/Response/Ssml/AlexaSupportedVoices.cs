namespace AlexaVoxCraft.Model.Response.Ssml;

/// <summary>
/// Provides constants for **Amazon Polly voice IDs supported by Alexa Skills**.
/// 
/// IMPORTANT:
/// Alexa Skills do NOT support all Amazon Polly voices.
/// The authoritative list is here:
/// https://developer.amazon.com/en-US/docs/alexa/custom-skills/speech-synthesis-markup-language-ssml-reference.html#supported-amazon-polly-voices
///
/// Only the voices in this file are guaranteed to work in Alexa Skills.
/// </summary>
public static class AlexaSupportedVoices
{
    // ----------------------------------------------------------------------
    // ENGLISH — UNITED STATES (en-US)
    // ----------------------------------------------------------------------
    public static class EnglishUS
    {
        public const string Ivy = "Ivy";           // Female
        public const string Joanna = "Joanna";     // Female
        public const string Kendra = "Kendra";     // Female
        public const string Kimberly = "Kimberly"; // Female
        public const string Salli = "Salli";       // Female

        public const string Joey = "Joey";         // Male
        public const string Justin = "Justin";     // Male
        public const string Matthew = "Matthew";   // Male
    }

    // ----------------------------------------------------------------------
    // ENGLISH — UNITED KINGDOM (en-GB)
    // ----------------------------------------------------------------------
    public static class EnglishGB
    {
        public const string Amy = "Amy";     // Female
        public const string Emma = "Emma";   // Female

        public const string Brian = "Brian"; // Male
    }

    // ----------------------------------------------------------------------
    // ENGLISH — AUSTRALIA (en-AU)
    // ----------------------------------------------------------------------
    public static class EnglishAU
    {
        public const string Nicole = "Nicole"; // Female
        public const string Russell = "Russell"; // Male
    }

    // ----------------------------------------------------------------------
    // ENGLISH — INDIA (en-IN)
    // ----------------------------------------------------------------------
    public static class EnglishIN
    {
        // NOTE: Aditi is bilingual (English + Hindi) and used in two locales
        public const string Aditi = "Aditi";     // Female
        public const string Raveena = "Raveena"; // Female
    }

    // ----------------------------------------------------------------------
    // ENGLISH — WALES (en-GB-WLS)
    // ----------------------------------------------------------------------
    public static class EnglishWLS
    {
        public const string Geraint = "Geraint"; // Male (Welsh English)
    }

    // ----------------------------------------------------------------------
    // FRENCH — CANADA (fr-CA)
    // ----------------------------------------------------------------------
    public static class FrenchCA
    {
        public const string Chantal = "Chantal"; // Female
    }

    // ----------------------------------------------------------------------
    // FRENCH — FRANCE (fr-FR)
    // ----------------------------------------------------------------------
    public static class FrenchFR
    {
        public const string Celine = "Celine";   // Female
        public const string Lea = "Lea";         // Female

        public const string Mathieu = "Mathieu"; // Male
    }

    // ----------------------------------------------------------------------
    // GERMAN — GERMANY (de-DE)
    // ----------------------------------------------------------------------
    public static class GermanDE
    {
        public const string Marlene = "Marlene"; // Female
        public const string Vicki = "Vicki";     // Female

        public const string Hans = "Hans";       // Male
    }

    // ----------------------------------------------------------------------
    // HINDI — INDIA (hi-IN)
    // ----------------------------------------------------------------------
    public static class HindiIN
    {
        // Aditi appears here as well
        public const string Aditi = "Aditi"; // Female
    }

    // ----------------------------------------------------------------------
    // ITALIAN — ITALY (it-IT)
    // ----------------------------------------------------------------------
    public static class ItalianIT
    {
        public const string Carla = "Carla";     // Female
        public const string Bianca = "Bianca";   // Female

        public const string Giorgio = "Giorgio"; // Male
    }

    // ----------------------------------------------------------------------
    // JAPANESE — JAPAN (ja-JP)
    // ----------------------------------------------------------------------
    public static class JapaneseJP
    {
        public const string Mizuki = "Mizuki"; // Female
        public const string Takumi = "Takumi"; // Male
    }

    // ----------------------------------------------------------------------
    // PORTUGUESE — BRAZIL (pt-BR)
    // ----------------------------------------------------------------------
    public static class PortugueseBR
    {
        public const string Vitoria = "Vitoria"; // Female
        public const string Camila = "Camila";   // Female

        public const string Ricardo = "Ricardo"; // Male
    }

    // ----------------------------------------------------------------------
    // SPANISH — MEXICO (es-MX)
    // ----------------------------------------------------------------------
    public static class SpanishMX
    {
        public const string Mia = "Mia"; // Female
    }

    // ----------------------------------------------------------------------
    // SPANISH — SPAIN (es-ES)
    // ----------------------------------------------------------------------
    public static class SpanishES
    {
        public const string Conchita = "Conchita"; // Female
        public const string Lucia = "Lucia";       // Female

        public const string Enrique = "Enrique";   // Male
    }

    // ----------------------------------------------------------------------
    // SPANISH — UNITED STATES (es-US)
    // ----------------------------------------------------------------------
    public static class SpanishUS
    {
        public const string Lupe = "Lupe";   // Female
        public const string Miguel = "Miguel"; // Male
    }
}
