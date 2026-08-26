using AlexaVoxCraft.MediatR.Lambda.Context;
using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;
using AlexaVoxCraft.Model.Response;
using Amazon.Lambda.Core;
using Compono;

namespace AlexaVoxCraft.MediatR.Lambda.Tests.TestKit;

public sealed class LambdaTestProfile : ICompositionProfile
{
    public void Configure(CompositionBuilder builder) => builder
        .UseBogus()
        .Register<SkillRequest>(() => CreateSkillRequest(new LaunchRequest
        {
            Type = "LaunchRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        }))
        .Register<LaunchRequest>(() => new LaunchRequest
        {
            Type = "LaunchRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        })
        .Register<SkillResponse>(() => new SkillResponse { Version = "1.0", Response = new ResponseBody() })
        .Register<SkillContext>(() => new DefaultSkillContext(CreateSkillRequest(new LaunchRequest
        {
            Type = "LaunchRequest",
            RequestId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Locale = "en-US"
        })))
        .Register<FakeLambdaContext>(() => new FakeLambdaContext())
        .Register<FakeSkillContextAccessor>(() => new FakeSkillContextAccessor());

    public static SkillRequest CreateSkillRequest(Request request) => new()
    {
        Context = new AlexaVoxCraft.Model.Request.Context { System = new AlexaSystem { Application = new Application { ApplicationId = "amzn1.ask.skill.test" } } },
        Request = request,
        Session = new Session { Attributes = new Dictionary<string, System.Text.Json.JsonElement>() }
    };
}

public sealed class FakeLambdaContext : ILambdaContext
{
    public string AwsRequestId { get; set; } = Guid.NewGuid().ToString();
    public IClientContext ClientContext { get; set; } = null!;
    public string FunctionName { get; set; } = "TestFunction";
    public string FunctionVersion { get; set; } = "1.0";
    public ICognitoIdentity Identity { get; set; } = null!;
    public string InvokedFunctionArn { get; set; } = "arn:aws:lambda:us-east-1:123456789012:function:TestFunction";
    public ILambdaLogger Logger { get; set; } = new FakeLambdaLogger();
    public string LogGroupName { get; set; } = "/aws/lambda/TestFunction";
    public string LogStreamName { get; set; } = "2023/10/15/[$LATEST]test";
    public int MemoryLimitInMB { get; set; } = 512;
    public TimeSpan RemainingTime { get; set; } = TimeSpan.FromMinutes(5);
}

public sealed class FakeLambdaLogger : ILambdaLogger
{
    public List<string> Messages { get; } = [];
    public void Log(string message) => Messages.Add(message);
    public void LogLine(string message) => Messages.Add(message);
}

public sealed class FakeSkillContextAccessor : ISkillContextAccessor
{
    private SkillContext? _skillContext;

    public List<SkillContext?> Assignments { get; } = [];

    public SkillContext? SkillContext
    {
        get => _skillContext;
        set
        {
            _skillContext = value;
            Assignments.Add(value);
        }
    }
}
