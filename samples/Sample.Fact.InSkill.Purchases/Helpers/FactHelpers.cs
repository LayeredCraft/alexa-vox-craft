using AlexaVoxCraft.InSkillPurchasing.Models;
using AlexaVoxCraft.Model.Request.Type;
using Sample.Fact.InSkill.Purchases.Data;

namespace Sample.Fact.InSkill.Purchases.Helpers;

public static class FactHelpers
{
    private static readonly string[] YesNoQuestions =
    [
        "Would you like another fact?",
        "Can I tell you another fact?",
        "Do you want to hear another fact?",
    ];

    private static readonly string[] Goodbyes =
    [
        "OK. Goodbye!",
        "Have a great day!",
        "Come back again soon!",
    ];

    public static string GetRandomFact(FactItem[] facts) =>
        facts[Random.Shared.Next(facts.Length)].Fact;

    public static string GetRandomYesNoQuestion() =>
        YesNoQuestions[Random.Shared.Next(YesNoQuestions.Length)];

    public static string GetRandomGoodbye() =>
        Goodbyes[Random.Shared.Next(Goodbyes.Length)];

    public static string GetSpeakableListOfProducts(IEnumerable<Product> products)
    {
        var names = products.Select(p => p.Name).ToList();
        return names.Count switch
        {
            0 => string.Empty,
            1 => names[0],
            _ => string.Join(", ", names[..^1]) + ", and " + names[^1],
        };
    }

    public static FactItem[] GetFilteredFacts(FactItem[] allFacts, Product[]? entitledProducts)
    {
        if (entitledProducts is null or { Length: 0 })
            return allFacts.Where(f => f.Type == "free").ToArray();

        var types = entitledProducts
            .Select(p => p.Name.ToLower().Replace(" pack", ""))
            .ToHashSet();
        types.Add("free");

        if (types.Contains("all access"))
            return allFacts;

        return allFacts.Where(f => types.Contains(f.Type)).ToArray();
    }

    public static string? GetResolvedSlotValue(IntentRequest request, string slotName) =>
        request.Intent?.Slots?.GetValueOrDefault(slotName)
            ?.Resolution?.Authorities?.FirstOrDefault()
            ?.Values?.FirstOrDefault()?.Value?.Name;

    public static string? GetSpokenSlotValue(IntentRequest request, string slotName) =>
        request.Intent?.Slots?.GetValueOrDefault(slotName)?.Value;
}