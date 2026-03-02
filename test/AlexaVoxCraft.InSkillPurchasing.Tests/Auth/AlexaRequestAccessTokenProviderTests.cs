using AlexaVoxCraft.InSkillPurchasing.Auth;
using AlexaVoxCraft.Model.Request;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Auth;

public sealed class AlexaRequestAccessTokenProviderTests
{
    [Fact]
    public async Task GetAccessTokenAsync_WhenTokenPresent_ReturnsToken()
    {
        const string expectedToken = "test-api-access-token";
        var skillRequest = new SkillRequest
        {
            Context = new Context { System = new AlexaSystem { ApiAccessToken = expectedToken } }
        };
        var sut = new AlexaRequestAccessTokenProvider(() => skillRequest);

        var result = await sut.GetAccessTokenAsync();

        result.Should().Be(expectedToken);
    }

    [Theory]
    [InlineAlexaVoxCraftAutoData(null)]
    [InlineAlexaVoxCraftAutoData("")]
    [InlineAlexaVoxCraftAutoData("   ")]
    public async Task GetAccessTokenAsync_WhenTokenNullOrWhitespace_ThrowsInvalidOperationException(string? token)
    {
        var skillRequest = new SkillRequest
        {
            Context = new Context { System = new AlexaSystem { ApiAccessToken = token! } }
        };
        var sut = new AlexaRequestAccessTokenProvider(() => skillRequest);

        var act = async () => await sut.GetAccessTokenAsync();

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ApiAccessToken*");
    }

    [Fact]
    public async Task GetAccessTokenAsync_WhenSkillRequestIsNull_ThrowsInvalidOperationException()
    {
        var sut = new AlexaRequestAccessTokenProvider(() => null);

        var act = async () => await sut.GetAccessTokenAsync();

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ApiAccessToken*");
    }

    [Fact]
    public void Constructor_WhenFactoryIsNull_ThrowsArgumentNullException()
    {
        var act = () => new AlexaRequestAccessTokenProvider(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("skillRequestFactory");
    }
}