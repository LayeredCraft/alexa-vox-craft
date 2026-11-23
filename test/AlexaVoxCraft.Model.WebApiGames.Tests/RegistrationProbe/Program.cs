using System.Text.Json;
using AlexaVoxCraft.Model;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.InSkillPurchasing;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Model.Response.Directive;
using AlexaVoxCraft.Model.Serialization;
using AlexaVoxCraft.Model.WebApiGames;

// Isolated-process registration probe. The registries in AlexaJsonOptions/RequestConverter/
// DirectiveConverter are global and have no public reset, so idempotence and order independence can
// only be proven in a fresh process. The first argument is an ordered comma-separated registration
// list (each of "webapigames", "apl", "isp"); the probe performs the registrations in that order and
// then exercises the Web API for Games round trips. It exits 0 on success and prints a diagnostic
// (and exits 1) on the first failed check.
var failures = new List<string>();

void Check(bool condition, string message)
{
    if (!condition)
    {
        failures.Add(message);
    }
}

var order = args.Length > 0 ? args[0].Split(',', StringSplitOptions.RemoveEmptyEntries) : [];
var aplRegistered = false;

foreach (var step in order)
{
    switch (step)
    {
        case "webapigames":
            WebApiGamesSupport.Add();
            break;
        case "apl":
            APLSupport.Add();
            aplRegistered = true;
            break;
        case "isp":
            InSkillPurchasingSupport.Add();
            break;
        default:
            Console.Error.WriteLine($"PROBE FAILURE: unknown registration '{step}'");
            return 1;
    }
}

var options = AlexaJsonOptions.DefaultOptions;

const string messageJson =
    """{"version":"1.0","request":{"type":"Alexa.Presentation.HTML.Message","requestId":"r","timestamp":"2024-05-01T12:00:00Z","locale":"en-US","message":{"action":"start","level":2}}}""";
var messageEnvelope = JsonSerializer.Deserialize<SkillRequest>(messageJson, options);
Check(messageEnvelope?.Request is MessageRequest, "MessageRequest did not resolve");
if (messageEnvelope?.Request is MessageRequest messageRequest)
{
    Check(
        messageRequest.Message.ValueKind == JsonValueKind.Object
        && messageRequest.Message.GetProperty("action").GetString() == "start",
        "MessageRequest payload was not preserved");
}

const string startJson =
    """{"type":"Alexa.Presentation.HTML.Start","request":{"uri":"https://example.com/game","method":"GET"},"configuration":{"timeoutInSeconds":30}}""";
Check(JsonSerializer.Deserialize<IDirective>(startJson, options) is StartDirective, "StartDirective did not resolve");

const string handleJson = """{"type":"Alexa.Presentation.HTML.HandleMessage","message":{"level":1}}""";
Check(
    JsonSerializer.Deserialize<IDirective>(handleJson, options) is HandleMessageDirective,
    "HandleMessageDirective did not resolve");

var response = ResponseBuilder.Empty();
response.Response.Directives.Add(new StartDirective(new StartRequest(new Uri("https://example.com/game"))));
var responseJson = JsonSerializer.Serialize(response, options);
Check(
    responseJson.Contains("Alexa.Presentation.HTML.Start", StringComparison.Ordinal),
    "Start directive did not serialize in the response");

var unknownRequestThrew = false;
try
{
    JsonSerializer.Deserialize<SkillRequest>(
        """{"version":"1.0","request":{"type":"Totally.Unknown","requestId":"r","timestamp":"2024-05-01T12:00:00Z","locale":"en-US"}}""",
        options);
}
catch (ArgumentOutOfRangeException)
{
    unknownRequestThrew = true;
}
catch (JsonException)
{
    unknownRequestThrew = true;
}

Check(unknownRequestThrew, "Unknown request type did not throw");

Check(
    JsonSerializer.Deserialize<IDirective>("""{"type":"Totally.Unknown","foo":"bar"}""", options) is JsonDirective,
    "Unknown directive did not fall back to JsonDirective");

if (aplRegistered)
{
    const string aplJson =
        """{"version":"1.0","context":{"System":{"apiAccessToken":"t","apiEndpoint":"e","application":{"applicationId":"a"},"user":{"userId":"u"},"device":{"deviceId":"d","supportedInterfaces":{"Alexa.Presentation.APL":{}}}}},"request":{"type":"Alexa.Presentation.APL.UserEvent","requestId":"r","timestamp":"2024-05-01T12:00:00Z","locale":"en-US","arguments":["a"]}}""";
    var aplEnvelope = JsonSerializer.Deserialize<APLSkillRequest>(aplJson, options);
    Check(aplEnvelope?.Request is UserEventRequest, "APL UserEventRequest did not resolve with APL registered");
}

if (failures.Count > 0)
{
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"PROBE FAILURE: {failure}");
    }

    return 1;
}

Console.WriteLine($"PROBE OK: order=[{string.Join(",", order)}]");
return 0;
