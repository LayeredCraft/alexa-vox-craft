// using System.Diagnostics;
// using System.Text;
// using AlexaVoxCraft.MediatR.Lambda.Serialization;
// using AlexaVoxCraft.MediatR.Observability;
// using AlexaVoxCraft.Model.Request;
// using AlexaVoxCraft.Model.Response;
// using AlexaVoxCraft.Model.Serialization;
// using AwesomeAssertions;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using OpenTelemetry;
// using OpenTelemetry.Metrics;
//
// namespace AlexaVoxCraft.MediatR.Lambda.Tests.Serialization;
//
// [Collection("DiagnosticsConfig Tests")]
// public class OtelAlexaLambdaSerializerTests : TestBase
// {
//     private readonly ActivityListener _activityListener;
//     private readonly List<Activity> _capturedActivities;
//     private readonly MeterProvider _meterProvider;
//     private readonly List<MetricSnapshot> _capturedMetrics;
//     private readonly ILogger<AlexaLambdaSerializer> _logger;
//     private readonly AlexaLambdaSerializer _serializer;
//
//     public OtelAlexaLambdaSerializerTests()
//     {
//         AlexaVoxCraftTelemetry.ResetForTesting();
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
//         // Setup serializer
//         _logger = Substitute.For<ILogger<AlexaLambdaSerializer>>();
//         _serializer = new AlexaLambdaSerializer(_logger, AlexaJsonOptions.DefaultOptions);
//     }
//
//     [Fact]
//     public void Deserialize_WithValidJson_RecordsRequestSerializationTelemetry()
//     {
//         // Arrange
//         var jsonContent = """
//             {
//                 "version": "1.0",
//                 "session": null,
//                 "context": {
//                     "System": {
//                         "application": { "applicationId": "test-app-id" },
//                         "user": { "userId": "test-user-id" },
//                         "device": { "deviceId": "test-device-id" }
//                     }
//                 },
//                 "request": {
//                     "type": "LaunchRequest",
//                     "requestId": "test-request-id",
//                     "locale": "en-US",
//                     "timestamp": "2023-01-01T00:00:00Z"
//                 }
//             }
//             """;
//         var jsonBytes = Encoding.UTF8.GetBytes(jsonContent);
//         using var stream = new MemoryStream(jsonBytes);
//         
//         // Act
//         var result = _serializer.Deserialize<SkillRequest>(stream);
//         
//         FlushMetrics();
//         
//         // Assert
//         result.Should().NotBeNull();
//         result.Request.Type.Should().Be("LaunchRequest");
//         
//         // Verify activity was created
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.OperationName.Should().Be(AlexaSpanNames.SerializationRequest);
//         activity.Status.Should().Be(ActivityStatusCode.Ok);
//         
//         // Verify activity tags
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionRequest));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.PayloadSize, jsonBytes.Length));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.CodeFunction, nameof(SkillRequest)));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.CodeNamespace, typeof(SkillRequest).Namespace!));
//         
//         // Verify metrics
//         var serializationDurationMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.SerializationDuration)
//             .ToList();
//         serializationDurationMetrics.Should().NotBeEmpty("Serialization duration histogram should be recorded");
//         
//         var payloadSizeMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.PayloadSize)
//             .ToList();
//         payloadSizeMetrics.Should().NotBeEmpty("Payload size histogram should be recorded");
//     }
//
//     [Fact]
//     public void Deserialize_WithInvalidJson_RecordsErrorTelemetry()
//     {
//         // Arrange
//         var invalidJsonContent = "{ invalid json content }";
//         var jsonBytes = Encoding.UTF8.GetBytes(invalidJsonContent);
//         using var stream = new MemoryStream(jsonBytes);
//         
//         // Act & Assert
//         var exception = Record.Exception(() => _serializer.Deserialize<SkillRequest>(stream));
//         exception.Should().NotBeNull();
//         
//         FlushMetrics();
//         
//         // Verify error activity was created
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.OperationName.Should().Be(AlexaSpanNames.SerializationRequest);
//         activity.Status.Should().Be(ActivityStatusCode.Error);
//         activity.StatusDescription.Should().NotBeNullOrEmpty();
//         
//         // Verify activity tags are still recorded
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionRequest));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.PayloadSize, jsonBytes.Length));
//     }
//
//     [Fact]
//     public void Serialize_WithValidObject_RecordsResponseSerializationTelemetry()
//     {
//         // Arrange
//         var skillResponse = new SkillResponse
//         {
//             Version = "1.0",
//             Response = new ResponseBody
//             {
//                 OutputSpeech = new PlainTextOutputSpeech { Text = "Hello World" },
//                 ShouldEndSession = true
//             }
//         };
//         using var stream = new MemoryStream();
//         
//         // Act
//         _serializer.Serialize(skillResponse, stream);
//         
//         FlushMetrics();
//         
//         // Assert
//         stream.Length.Should().BeGreaterThan(0);
//         
//         // Verify activity was created
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.OperationName.Should().Be(AlexaSpanNames.SerializationResponse);
//         activity.Status.Should().Be(ActivityStatusCode.Ok);
//         
//         // Verify activity tags
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionResponse));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.PayloadSize, stream.Length));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.CodeFunction, nameof(SkillResponse)));
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.CodeNamespace, typeof(SkillResponse).Namespace!));
//         
//         // Verify metrics
//         var serializationDurationMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.SerializationDuration)
//             .ToList();
//         serializationDurationMetrics.Should().NotBeEmpty("Serialization duration histogram should be recorded");
//         
//         var payloadSizeMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.PayloadSize)
//             .ToList();
//         payloadSizeMetrics.Should().NotBeEmpty("Payload size histogram should be recorded");
//     }
//
//     [Fact]
//     public void Serialize_WithNonSerializableObject_RecordsErrorTelemetry()
//     {
//         // Arrange
//         var circularObject = new CircularReferenceObject();
//         circularObject.SelfReference = circularObject; // Create circular reference
//         
//         using var stream = new MemoryStream();
//         
//         // Act & Assert
//         var exception = Record.Exception(() => _serializer.Serialize(circularObject, stream));
//         exception.Should().NotBeNull();
//         
//         FlushMetrics();
//         
//         // Verify error activity was created
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.OperationName.Should().Be(AlexaSpanNames.SerializationResponse);
//         activity.Status.Should().Be(ActivityStatusCode.Error);
//         activity.StatusDescription.Should().NotBeNullOrEmpty();
//         
//         // Verify activity tags are still recorded
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.SerializationDirection, AlexaSemanticValues.SerializationDirectionResponse));
//     }
//
//     [Fact]
//     public void Serialize_WithReadOnlyStream_RecordsErrorTelemetry()
//     {
//         // Arrange
//         var skillResponse = new SkillResponse
//         {
//             Version = "1.0",
//             Response = new ResponseBody
//             {
//                 OutputSpeech = new PlainTextOutputSpeech { Text = "Hello World" },
//                 ShouldEndSession = true
//             }
//         };
//         
//         var readOnlyBytes = new byte[1024];
//         using var readOnlyStream = new MemoryStream(readOnlyBytes, writable: false);
//         
//         // Act & Assert
//         var exception = Record.Exception(() => _serializer.Serialize(skillResponse, readOnlyStream));
//         exception.Should().NotBeNull();
//         
//         FlushMetrics();
//         
//         // Verify error activity was created
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.OperationName.Should().Be(AlexaSpanNames.SerializationResponse);
//         activity.Status.Should().Be(ActivityStatusCode.Error);
//         activity.StatusDescription.Should().NotBeNullOrEmpty();
//     }
//
//     [Theory]
//     [InlineData(1024, "Small payload")]
//     [InlineData(65536, "Large payload")]
//     [InlineData(1048576, "Very large payload")]
//     public void Serialize_WithVariousPayloadSizes_RecordsCorrectPayloadSizeMetrics(int approximateSize, string description)
//     {
//         // Arrange
//         var largeText = new string('A', approximateSize);
//         var skillResponse = new SkillResponse
//         {
//             Version = "1.0",
//             Response = new ResponseBody
//             {
//                 OutputSpeech = new PlainTextOutputSpeech { Text = largeText },
//                 ShouldEndSession = true
//             }
//         };
//         using var stream = new MemoryStream();
//         
//         // Act
//         _serializer.Serialize(skillResponse, stream);
//         
//         FlushMetrics();
//         
//         // Assert
//         stream.Length.Should().BeGreaterThan(approximateSize / 2, $"Stream should contain {description}");
//         
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.PayloadSize, stream.Length));
//         
//         // Verify payload size metrics include correct tags
//         var payloadSizeMetrics = _capturedMetrics
//             .Where(m => m.Name == AlexaMetricNames.PayloadSize)
//             .ToList();
//         payloadSizeMetrics.Should().NotBeEmpty("Payload size histogram should be recorded for " + description);
//     }
//
//     [Fact]
//     public void Deserialize_WithEmptyStream_RecordsZeroPayloadSize()
//     {
//         // Arrange
//         using var emptyStream = new MemoryStream();
//         
//         // Act & Assert
//         var exception = Record.Exception(() => _serializer.Deserialize<SkillRequest>(emptyStream));
//         exception.Should().NotBeNull(); // Should fail to deserialize empty stream
//         
//         FlushMetrics();
//         
//         // Verify activity records zero payload size
//         var activity = _capturedActivities.Should().ContainSingle().Subject;
//         activity.TagObjects.Should().ContainEquivalentOf(
//             new KeyValuePair<string, object?>(AlexaSemanticAttributes.PayloadSize, 0L));
//     }
//
//     /// <summary>
//     /// Forces collection of metrics by triggering the meter provider to export.
//     /// Call this before asserting on captured metrics.
//     /// </summary>
//     private void FlushMetrics()
//     {
//         _meterProvider.ForceFlush(1000); // 1000ms timeout
//     }
// }
//
// /// <summary>
// /// Test class for creating circular reference serialization errors.
// /// </summary>
// public class CircularReferenceObject
// {
//     public CircularReferenceObject? SelfReference { get; set; }
// }