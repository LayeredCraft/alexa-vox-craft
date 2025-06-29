namespace AlexaVoxCraft.Model.Apl.Components;

public static class APLComponentExtensions
{
    public static void AddResponsiveDesign(this APLDocument document)
    {
        Import.AlexaLayouts.Into(document);
    }
}