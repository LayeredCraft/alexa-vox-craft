using AlexaVoxCraft.TestKit.RequestSpecifications;
using Amazon.Lambda.Core;
using AutoFixture.Kernel;
using NSubstitute;

namespace AlexaVoxCraft.TestKit.SpecimenBuilders;

/// <summary>
/// Custom specimen builder for ILambdaContext based on parameter naming conventions.
/// </summary>
public class LambdaContextSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public LambdaContextSpecimenBuilder() : this(new LambdaContextSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var lambdaContext = Substitute.For<ILambdaContext>();
        lambdaContext.AwsRequestId.Returns(Guid.NewGuid().ToString());
        lambdaContext.FunctionName.Returns("TestFunction");
        lambdaContext.FunctionVersion.Returns("1.0");
        lambdaContext.InvokedFunctionArn.Returns("arn:aws:lambda:us-east-1:123456789012:function:TestFunction");
        lambdaContext.MemoryLimitInMB.Returns(512);
        lambdaContext.RemainingTime.Returns(TimeSpan.FromMinutes(5));
        lambdaContext.LogGroupName.Returns("/aws/lambda/TestFunction");
        lambdaContext.LogStreamName.Returns("2023/10/15/[$LATEST]test");

        return lambdaContext;
    }
}