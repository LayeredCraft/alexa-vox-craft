namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public sealed class StartConfigurationTests
{
    [Fact]
    public void DefaultsToThirtySeconds()
    {
        new StartConfiguration().TimeoutInSeconds.Should().Be(30);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(300)]
    public void AcceptsTimeoutsUpToTheMaximum(int timeoutInSeconds)
    {
        new StartConfiguration(timeoutInSeconds).TimeoutInSeconds.Should().Be(timeoutInSeconds);
    }

    [Theory]
    [InlineData(-1)]
    public void DoesNotInventAMinimum(int timeoutInSeconds)
    {
        new StartConfiguration(timeoutInSeconds).TimeoutInSeconds.Should().Be(timeoutInSeconds);
    }

    [Theory]
    [InlineData(301)]
    [InlineData(int.MaxValue)]
    public void RejectsTimeoutsAboveTheMaximum(int timeoutInSeconds)
    {
        var act = () => new StartConfiguration(timeoutInSeconds);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
