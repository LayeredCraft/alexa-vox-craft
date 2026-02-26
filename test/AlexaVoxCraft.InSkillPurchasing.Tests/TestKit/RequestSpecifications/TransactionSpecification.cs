using System.Reflection;
using AlexaVoxCraft.InSkillPurchasing.Models;
using AutoFixture.Kernel;

namespace AlexaVoxCraft.InSkillPurchasing.Tests.TestKit.RequestSpecifications;

/// <summary>
/// Specification that determines if a request is for a Transaction or TransactionResponse type.
/// </summary>
public class TransactionSpecification : IRequestSpecification
{
    public bool IsSatisfiedBy(object request)
    {
        var type = request switch
        {
            Type t => t,
            ParameterInfo p => p.ParameterType,
            _ => null
        };
        return type == typeof(Transaction) || type == typeof(TransactionResponse);
    }
}