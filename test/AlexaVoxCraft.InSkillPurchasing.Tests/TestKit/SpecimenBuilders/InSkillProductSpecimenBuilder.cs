using System.Reflection;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.SpecimenBuilders;

/// <summary>
/// AutoFixture specimen builder that creates realistic Product and ProductResponse instances.
/// </summary>
public sealed class InSkillProductSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public InSkillProductSpecimenBuilder() : this(new InSkillProductSpecification())
    {
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (!requestSpecification.IsSatisfiedBy(request))
            return new NoSpecimen();

        var requestType = request switch
        {
            Type t => t,
            ParameterInfo p => p.ParameterType,
            _ => throw new ArgumentException("Invalid request type", nameof(request))
        };

        return requestType == typeof(ProductResponse)
            ? CreateProductResponse(context)
            : CreateProduct(context);
    }

    private static Product CreateProduct(ISpecimenContext context) =>
        new(
            ProductId: $"amzn1.adg.product.{context.Create<string>()}",
            Name: context.Create<string>(),
            Type: ProductType.ENTITLEMENT,
            Summary: context.Create<string>(),
            Purchasable: Purchasable.PURCHASABLE,
            Entitled: Entitled.NOT_ENTITLED,
            EntitlementReason: EntitlementReason.NOT_PURCHASED,
            ReferenceName: context.Create<string>(),
            ActiveEntitlementCount: Math.Abs(context.Create<int>() % 10),
            PurchaseMode: PurchaseMode.TEST)
        {
            StartTime = DateTimeOffset.UtcNow.AddDays(-7),
            EndTime = DateTimeOffset.UtcNow.AddDays(30)
        };

    private static ProductResponse CreateProductResponse(ISpecimenContext context) =>
        new(
            Products: [CreateProduct(context), CreateProduct(context)],
            IsTruncated: false,
            NextToken: null);
}