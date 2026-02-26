using System.Reflection;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.RequestSpecifications;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.SpecimenBuilders;

/// <summary>
/// AutoFixture specimen builder that creates realistic Transaction and TransactionResponse instances.
/// </summary>
public sealed class TransactionSpecimenBuilder(IRequestSpecification requestSpecification) : ISpecimenBuilder
{
    public TransactionSpecimenBuilder() : this(new TransactionSpecification())
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

        return requestType == typeof(TransactionResponse)
            ? CreateTransactionResponse(context)
            : CreateTransaction(context);
    }

    private static Transaction CreateTransaction(ISpecimenContext context) =>
        new(
            Status: TransactionStatus.APPROVED_BY_PARENT,
            ProductId: $"amzn1.adg.product.{context.Create<string>()}",
            CreatedTime: DateTimeOffset.UtcNow.AddDays(-1),
            LastModifiedTime: DateTimeOffset.UtcNow);

    private static TransactionResponse CreateTransactionResponse(ISpecimenContext context) =>
        new(
            Results: [CreateTransaction(context), CreateTransaction(context)],
            Metadata: new TransactionResponseMetadata(
                new TransactionResultSet(context.Create<string>())));
}