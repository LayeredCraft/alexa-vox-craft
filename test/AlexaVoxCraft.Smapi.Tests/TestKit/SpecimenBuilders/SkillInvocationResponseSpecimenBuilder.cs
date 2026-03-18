using AlexaVoxCraft.Model.Response;
using AlexaVoxCraft.Smapi.Models.Invocation;
using AlexaVoxCraft.Smapi.Tests.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.Smapi.Tests.TestKit.SpecimenBuilders;

public sealed class SkillInvocationResponseSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public SkillInvocationResponseSpecimenBuilder() : this(new SkillInvocationResponseSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        return new SkillInvocationResponse<SkillResponse>
        {
            Status = "SUCCESSFUL",
            Result = new SkillInvocationResult<SkillResponse>
            {
                SkillExecutionInfo = new SkillExecutionInfo<SkillResponse>
                {
                    Metrics = new SkillExecutionMetrics
                    {
                        SkillExecutionTimeInMilliseconds = context.Create<int>()
                    }
                }
            }
        };
    }
}