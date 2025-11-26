using AlexaVoxCraft.Model.Response.Ssml;

namespace AlexaVoxCraft.MediatR.Response;

/// <summary>
/// Provides extension methods for building Alexa skill responses with SSML voice support.
/// </summary>
/// <remarks>
/// These extensions allow wrapping text, SSML nodes, or collections of SSML elements
/// in an Amazon Polly voice. Use <see cref="AlexaSupportedVoices"/> for available voice name constants
/// that are supported by Alexa Skills.
/// </remarks>
public static class ResponseExtensions
{
    extension(string text)
    {
        /// <summary>
        /// Wraps the text in an SSML <c>&lt;voice&gt;</c> element using the specified Amazon Polly voice.
        /// </summary>
        /// <param name="voiceName">The Amazon Polly voice name to use. See <see cref="AlexaSupportedVoices"/> for available voices.</param>
        /// <returns>An <see cref="ISsml"/> element containing the text wrapped in a voice element.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> or <paramref name="voiceName"/> is null or whitespace.</exception>
        public ISsml WithVoice(string voiceName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);
            ArgumentException.ThrowIfNullOrWhiteSpace(voiceName);

            return new PlainText(text).WithVoice(voiceName);
        }
    }

    extension(ISsml node)
    {
        /// <summary>
        /// Wraps the SSML element in an SSML <c>&lt;voice&gt;</c> element using the specified Amazon Polly voice.
        /// </summary>
        /// <param name="voiceName">The Amazon Polly voice name to use. See <see cref="AlexaSupportedVoices"/> for available voices.</param>
        /// <returns>An <see cref="ISsml"/> element containing the node wrapped in a voice element.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="voiceName"/> is null or whitespace.</exception>
        public ISsml WithVoice(string voiceName)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentException.ThrowIfNullOrWhiteSpace(voiceName);

            return new Voice(voiceName)
            {
                Elements = [node]
            };
        }
    }

    extension(IEnumerable<ISsml> nodes)
    {
        /// <summary>
        /// Wraps the collection of SSML elements in an SSML <c>&lt;voice&gt;</c> element using the specified Amazon Polly voice.
        /// </summary>
        /// <param name="voiceName">The Amazon Polly voice name to use. See <see cref="AlexaSupportedVoices"/> for available voices.</param>
        /// <returns>An <see cref="ISsml"/> element containing all nodes wrapped in a single voice element.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nodes"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="voiceName"/> is null or whitespace.</exception>
        public ISsml WithVoice(string voiceName)
        {
            ArgumentNullException.ThrowIfNull(nodes);
            ArgumentException.ThrowIfNullOrWhiteSpace(voiceName);

            return new Voice(voiceName)
            {
                Elements = nodes.ToList()
            };
        }
    }

}