using System.Text.Json;
using AlexaVoxCraft.InSkillPurchasing.Models;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.Models;

public class ProductTests : TestBase<ProductTests>
{
    [Fact]
    public async Task Product_Deserializes()
    {
        var json = Fx("Models/InSkillProduct.json");
        var product = JsonSerializer.Deserialize<Product>(json, ClientOptions);
        
        product.Should().NotBeNull();

        await TestHelper.VerifyRequestObject(product);
    }

    [Fact]
    public async Task ProductResponse_Deserializes()
    {
        var  json = Fx("Models/InSkillProductsResponse.json");
        var response = JsonSerializer.Deserialize<ProductResponse>(json, ClientOptions);
        
        response.Should().NotBeNull();
        response.Products.Should().HaveCount(1);
        
        await TestHelper.VerifyRequestObject(response);
    }
}