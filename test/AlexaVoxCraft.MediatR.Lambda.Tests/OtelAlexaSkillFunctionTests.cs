// using System.Diagnostics;
// using AlexaVoxCraft.MediatR.Lambda.Abstractions;
// using AlexaVoxCraft.MediatR.Observability;
// using AlexaVoxCraft.Model.Request;
// using AlexaVoxCraft.Model.Request.Type;
// using AlexaVoxCraft.Model.Response;
// using Amazon.Lambda.Core;
// using AwesomeAssertions;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using NSubstitute;
// using OpenTelemetry;
// using OpenTelemetry.Metrics;
//
// namespace AlexaVoxCraft.MediatR.Lambda.Tests;
//
// [Collection("DiagnosticsConfig Tests")]
// public class OtelAlexaSkillFunctionTests : TestBase, IDisposable
// {
//     private readonly ActivityListener _activityListener;
//     private readonly List<Activity> _capturedActivities;
//     private readonly MeterProvider _meterProvider;
//     private readonly List<MetricSnapshot> _capturedMetrics;
//     private readonly TestSkillFunction _skillFunction;
//     private readonly ILambdaContext _lambdaContext;
//     private readonly SkillRequest _skillRequest;
//
//     public OtelAlexaSkillFunctionTests()
//     {
//         AlexaVoxCraftTelemetry.ResetForTesting();
//         
//         // Setup activity capturing
//         _capturedActivities = [];
//         _activityListener = new ActivityListener
//         {
//             ShouldListenTo = source => source.Name.Contains(AlexaVoxCraftTelemetry.ActivitySourceName),
//             Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
//             ActivityStarted = activity => _capturedActivities.Add(activity),
//         };
//         ActivitySource.AddActivityListener(_activityListener);
//
//         // Setup metrics capturing
//         _capturedMetrics = [];
//         _meterProvider = Sdk.CreateMeterProviderBuilder()
//             .AddMeter(AlexaVoxCraftTelemetry.MeterName)
//             .AddInMemoryExporter(_capturedMetrics)
//             .Build();
//
//         // Setup test objects
//         _skillFunction = new TestSkillFunction();
//         _lambdaContext = CreateLambdaContext();
//         _skillRequest = CreateSkillRequest();
//     }
//
//     [Fact]
//     public async Task FunctionHandlerAsync_WithValidRequest_RecordsLambdaExecutionTelemetry()
//     {
//         // Act
//         var result = await _skillFunction.FunctionHandlerAsync(_skillRequest, _lambdaContext);
//         
//         FlushMetrics();
//         
//         // Assert
//         result.Should().NotBeNull();
//         result.Version.Should().Be("1.0");
//         
//         // Verify activity was created
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.OperationName.Should().Be(AlexaSpanNames.LambdaExecution);
//         activity.Kind.Should().Be(ActivityKind.Internal);
//         activity.Status.Should().Be(ActivityStatusCode.Ok);
//         
//         // Verify FaaS semantic attributes
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.FaasName, "test-function"));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.FaasVersion, "1.0"));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.AwsLambdaRequestId, "test-request-id"));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.AwsLambdaMemoryLimit, 512));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.ApplicationId, "test-app-id"));
//         
//         // Verify remaining time was recorded (should be positive)
//         var remainingTimeTag = activity.TagObjects.FirstOrDefault(t => 
//             t.Key == AlexaSemanticAttributes.AwsLambdaRemainingTime);
//         remainingTimeTag.Should().NotBeNull();
//         ((double)remainingTimeTag.Value!).Should().BePositive();
//         
//         // Verify Lambda execution duration metrics
//         var lambdaDurationMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.LambdaDuration)
//             .ToList();
//         lambdaDurationMetrics.Should().NotBeEmpty("Lambda duration histogram should be recorded");
//         
//         // Verify Lambda memory usage metrics
//         var memoryUsageMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.LambdaMemoryUsed)
//             .ToList();
//         memoryUsageMetrics.Should().NotBeEmpty("Lambda memory usage histogram should be recorded");
//     }
//
//     [Fact]
//     public async Task FunctionHandlerAsync_OnColdStart_RecordsColdStartTelemetry()
//     {
//         // Arrange - Ensure this is treated as a cold start
//         AlexaVoxCraftTelemetry.ResetForTesting(); // This ensures IsColdStart() returns true
//         
//         // Act
//         var result = await _skillFunction.FunctionHandlerAsync(_skillRequest, _lambdaContext);
//         
//         FlushMetrics();
//         
//         // Assert
//         result.Should().NotBeNull();
//         
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.FaasColdStart, AlexaSemanticValues.True));
//     }
//
//     [Fact]
//     public async Task FunctionHandlerAsync_OnWarmStart_DoesNotRecordColdStartAttribute()
//     {
//         // Arrange - Simulate warm start by calling IsColdStart() once to mark it as no longer cold
//         AlexaVoxCraftTelemetry.IsColdStart(); // First call marks it as no longer cold
//         
//         // Act
//         var result = await _skillFunction.FunctionHandlerAsync(_skillRequest, _lambdaContext);
//         
//         FlushMetrics();
//         
//         // Assert
//         result.Should().NotBeNull();
//         
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.TagObjects.Should().NotContain(t => 
//             t.Key == AlexaSemanticAttributes.FaasColdStart);
//     }
//
//     [Fact]
//     public async Task FunctionHandlerAsync_WithDifferentMemoryLimits_RecordsCorrectMemoryMetrics()
//     {
//         // Arrange
//         var highMemoryContext = Substitute.For<ILambdaContext>();
//         highMemoryContext.FunctionName.Returns("test-function");
//         highMemoryContext.FunctionVersion.Returns("1.0");
//         highMemoryContext.AwsRequestId.Returns("test-request-id");
//         highMemoryContext.MemoryLimitInMB.Returns(1024); // Higher memory limit
//         highMemoryContext.RemainingTime.Returns(TimeSpan.FromMinutes(5));
//         
//         // Act
//         var result = await _skillFunction.FunctionHandlerAsync(_skillRequest, highMemoryContext);
//         
//         FlushMetrics();
//         
//         // Assert
//         result.Should().NotBeNull();
//         
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.AwsLambdaMemoryLimit, 1024));
//         
//         var memoryUsageMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.LambdaMemoryUsed)
//             .ToList();
//         memoryUsageMetrics.Should().NotBeEmpty();
//     }
//
//     [Fact]
//     public async Task FunctionHandlerAsync_WithException_RecordsErrorTelemetry()
//     {
//         // Arrange
//         var throwingFunction = new ThrowingSkillFunction();
//         
//         // Act & Assert
//         var exception = await Record.ExceptionAsync(() => 
//             throwingFunction.FunctionHandlerAsync(_skillRequest, _lambdaContext));
//         exception.Should().NotBeNull();
//         
//         FlushMetrics();
//         
//         // Verify error activity was created
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.OperationName.Should().Be(AlexaSpanNames.LambdaExecution);
//         activity.Status.Should().Be(ActivityStatusCode.Error);
//         activity.StatusDescription.Should().NotBeNullOrEmpty();
//         
//         // Verify exception event was added
//         var exceptionEvent = activity.Events.Should().ContainSingle(e => 
//             e.Name == AlexaEventNames.Exception).Subject;
//         
//         var exceptionAttributes = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
//         exceptionAttributes.Should().ContainKey(AlexaSemanticAttributes.ExceptionType);
//         exceptionAttributes.Should().ContainKey(AlexaSemanticAttributes.ExceptionMessage);
//         exceptionAttributes.Should().ContainKey(AlexaSemanticAttributes.ExceptionStackTrace);
//         
//         exceptionAttributes[AlexaSemanticAttributes.ExceptionType].Should().Be(typeof(InvalidOperationException).FullName);
//         exceptionAttributes[AlexaSemanticAttributes.ExceptionMessage].Should().Be("Test exception");
//     }
//
//     [Theory]
//     [InlineData("test-app-1", "req-1", "func-1", "v1.0")]
//     [InlineData("test-app-2", "req-2", "func-2", "v2.0")]
//     [InlineData("test-app-3", "req-3", "func-3", "v3.0")]
//     public async Task FunctionHandlerAsync_WithDifferentContexts_RecordsCorrectSemanticAttributes(
//         string applicationId, string requestId, string functionName, string functionVersion)
//     {
//         // Arrange
//         var customRequest = CreateSkillRequest();
//         customRequest.Context.System.Application.ApplicationId = applicationId;
//         
//         var customContext = Substitute.For<ILambdaContext>();
//         customContext.FunctionName.Returns(functionName);
//         customContext.FunctionVersion.Returns(functionVersion);
//         customContext.AwsRequestId.Returns(requestId);
//         customContext.MemoryLimitInMB.Returns(512);
//         customContext.RemainingTime.Returns(TimeSpan.FromMinutes(2));
//         
//         // Act
//         var result = await _skillFunction.FunctionHandlerAsync(customRequest, customContext);
//         
//         FlushMetrics();
//         
//         // Assert
//         result.Should().NotBeNull();
//         
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.ApplicationId, applicationId));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.FaasName, functionName));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.FaasVersion, functionVersion));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.AwsLambdaRequestId, requestId));
//     }
//
//     [Fact]
//     public async Task FunctionHandlerAsync_RecordsExecutionTimingMetrics()
//     {
//         // Act
//         var startTime = DateTimeOffset.UtcNow;
//         var result = await _skillFunction.FunctionHandlerAsync(_skillRequest, _lambdaContext);
//         var endTime = DateTimeOffset.UtcNow;
//         
//         FlushMetrics();
//         
//         // Assert
//         result.Should().NotBeNull();
//         
//         // Verify timing metrics were recorded
//         var durationMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.LambdaDuration)
//             .ToList();
//         durationMetrics.Should().NotBeEmpty("Lambda duration should be recorded");
//         
//         // Verify activity duration is reasonable
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.Duration.Should().BeGreaterThan(TimeSpan.Zero);
//         activity.Duration.Should().BeLessThan(endTime - startTime + TimeSpan.FromSeconds(1)); // Allow some tolerance
//     }
//
//     private ILambdaContext CreateLambdaContext()
//     {
//         var context = Substitute.For<ILambdaContext>();
//         context.FunctionName.Returns("test-function");
//         context.FunctionVersion.Returns("1.0");
//         context.AwsRequestId.Returns("test-request-id");
//         context.MemoryLimitInMB.Returns(512);
//         context.RemainingTime.Returns(TimeSpan.FromMinutes(5));
//         return context;
//     }
//
//     private SkillRequest CreateSkillRequest()
//     {
//         return new SkillRequest
//         {
//             Version = "1.0",
//             Context = new AlexaVoxCraft.Model.Request.Context
//             {
//                 System = new AlexaSystem
//                 {
//                     Application = new Application
//                     {
//                         ApplicationId = "test-app-id"
//                     },
//                     User = new User
//                     {
//                         UserId = "test-user-id"
//                     },
//                     Device = new Device
//                     {
//                         DeviceID = "test-device-id"
//                     }
//                 }
//             },
//             Request = new LaunchRequest
//             {
//                 Type = "LaunchRequest",
//                 RequestId = "test-request-id",
//                 Locale = "en-US",
//                 Timestamp = DateTime.UtcNow
//             }
//         };
//     }
//
//     private void FlushMetrics()
//     {
//         _meterProvider.ForceFlush(1000); // 1000ms timeout
//     }
//
//     public void Dispose()
//     {
//         _activityListener?.Dispose();
//         _meterProvider?.Dispose();
//     }
// }
//
// public class TestSkillFunction : AlexaSkillFunction<SkillRequest, SkillResponse>
// {
//     protected override void Init(IHostBuilder builder)
//     {
//         builder.ConfigureServices((_, services) =>
//         {
//             services.AddSingleton<HandlerDelegate<SkillRequest, SkillResponse>>((_, _) =>
//             {
//                 return Task.FromResult(new SkillResponse
//                 {
//                     Version = "1.0",
//                     Response = new ResponseBody
//                     {
//                         OutputSpeech = new PlainTextOutputSpeech { Text = "Hello World" },
//                         ShouldEndSession = true
//                     }
//                 });
//             });
//         });
//     }
// }
//
// public class ThrowingSkillFunction : AlexaSkillFunction<SkillRequest, SkillResponse>
// {
//     protected override void Init(IHostBuilder builder)
//     {
//         builder.ConfigureServices((_, services) =>
//         {
//             services.AddSingleton<HandlerDelegate<SkillRequest, SkillResponse>>((_, _) =>
//             {
//                 throw new InvalidOperationException("Test exception");
//             });
//         });
//     }
// }