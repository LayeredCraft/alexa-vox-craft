using System.Text.Json;
using AlexaVoxCraft.InSkillPurchasing.Models;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Models;

public class TransactionTests : TestBase<TransactionTests>
{
    [Fact]
    public async Task TransactionResponse_Deserializes()
    {
        var json = Fx("Models/TransactionResponse.json");
        var response = JsonSerializer.Deserialize<TransactionResponse>(json, ClientOptions);

        response.Should().NotBeNull();

        await TestHelper.VerifyRequestObject(response);
    }
}