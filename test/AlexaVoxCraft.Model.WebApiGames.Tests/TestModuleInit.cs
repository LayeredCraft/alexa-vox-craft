using System.Runtime.CompilerServices;
using AlexaVoxCraft.Model.Apl;
using AlexaVoxCraft.Model.InSkillPurchasing;

namespace AlexaVoxCraft.Model.WebApiGames.Tests;

public static class TestModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        WebApiGamesSupport.Add();
        APLSupport.Add();
        InSkillPurchasingSupport.Add();
    }
}
