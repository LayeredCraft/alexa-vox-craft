using AlexaVoxCraft.Model.Request;
using AlexaVoxCraft.Model.Request.Type;

namespace AlexaVoxCraft.Model.Tests.Request;

/// <summary>
/// Coverage for <see cref="RequestVerification.RequestTimestampWithinTolerance(SkillRequest)"/>,
/// the replay-attack guard that checks a request's timestamp against the current time.
/// </summary>
public sealed class RequestVerificationTests
{
    [Fact]
    public void RecentTimestamp_IsWithinTolerance()
    {
        var request = new SkillRequest { Request = new LaunchRequest { Timestamp = DateTime.Now.AddMinutes(1) } };

        RequestVerification.RequestTimestampWithinTolerance(request).Should().BeTrue();
    }

    [Fact]
    public void OldTimestamp_IsOutsideTolerance()
    {
        var request = new SkillRequest { Request = new LaunchRequest { Timestamp = DateTime.Now.AddMinutes(3) } };

        RequestVerification.RequestTimestampWithinTolerance(request).Should().BeFalse();
    }
}
