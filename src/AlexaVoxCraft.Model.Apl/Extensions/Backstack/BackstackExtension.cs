namespace AlexaVoxCraft.Model.Apl.Extensions.Backstack;

public class BackstackExtension : APLExtension
{
    public const string URL = "aplext:backstack:10";

    public BackstackExtension()
    {
        Uri = URL;
    }

    public BackstackExtension(string name) : this()
    {
        Name = name;
    }
}