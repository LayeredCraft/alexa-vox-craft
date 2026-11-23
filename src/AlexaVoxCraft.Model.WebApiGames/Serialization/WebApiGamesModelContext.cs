using System.Text.Json.Serialization;

namespace AlexaVoxCraft.Model.WebApiGames.Serialization;

// Root types only - see AlexaVoxCraft.Model.Serialization.ModelContext for the same convention.
// StartDirective and HandleMessageDirective are DirectiveConverter dispatch targets
// (RegisterDirectiveDerivedType) and MessageRequest is a RequestConverter dispatch target
// (MessageRequestTypeResolver). Both converters erase to the value's runtime type through
// JsonSerializer.Serialize/Deserialize(value.GetType()/resolved type, options), so every such
// runtime target needs an explicit entry here. StartRequest, StartConfiguration and HTMLTransformer
// are reachable only as properties of StartDirective and are generated automatically.
[JsonSerializable(typeof(StartDirective))]
[JsonSerializable(typeof(HandleMessageDirective))]
[JsonSerializable(typeof(MessageRequest))]
internal partial class WebApiGamesModelContext : JsonSerializerContext
{
}
