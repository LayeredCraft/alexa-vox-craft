using System.Runtime.CompilerServices;

namespace AlexaVoxCraft.Model.Apl.Tests;

public static class TestModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Centralize all snapshots under: <project>/Snapshots
        UseProjectRelativeDirectory("Snapshots");
        
        // Make Verify(object) JSON match your style
        VerifierSettings.UseStrictJson();
        
        VerifierSettings.ScrubEmptyLines();
        VerifierSettings.ScrubLinesWithReplace(line => line.Replace("<!--!-->", ""));
        APLSupport.Add();
    }
}