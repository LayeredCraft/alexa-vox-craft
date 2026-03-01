using System.Text.Json;

namespace AlexaVoxCraft.MediatR.Attributes;

/// <summary>
/// Extension methods for working with typed values in an Alexa attribute dictionary.
/// </summary>
public static class AttributeDictionaryExtensions
{
    /// <param name="attributes">The attribute dictionary.</param>
    extension(IDictionary<string, object> attributes)
    {
        /// <summary>
        /// Attempts to retrieve and deserialize a typed value from the attribute dictionary.
        /// When session attributes were written by <c>TypedAttributeDictionaryConverter</c>, the value is
        /// already the correct CLR type and is returned directly without any serialization overhead.
        /// Falls back to a UTF-8 serialize/deserialize round-trip for values written without type
        /// discriminators (e.g., set by Alexa directly, or stored before the converter was introduced).
        /// </summary>
        /// <typeparam name="T">The target type to deserialize into.</typeparam>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The value, or <see langword="default"/> if the key was not found.</param>
        /// <returns><see langword="true"/> if the key existed; otherwise <see langword="false"/>.</returns>
        public bool TryGetAttribute<T>(string key, out T? value)
        {
            if (!attributes.TryGetValue(key, out var raw))
            {
                value = default;
                return false;
            }

            if (raw is T typed)
            {
                value = typed;
                return true;
            }

            // Fallback for legacy values without a type discriminator.
            value = JsonSerializer.Deserialize<T>(JsonSerializer.SerializeToUtf8Bytes(raw));
            return true;
        }

        /// <summary>
        /// Stores a typed value in the attribute dictionary under the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The value to store.</param>
        public void SetAttribute<T>(string key, T value)
        {
            attributes[key] = value!;
        }
    }
}