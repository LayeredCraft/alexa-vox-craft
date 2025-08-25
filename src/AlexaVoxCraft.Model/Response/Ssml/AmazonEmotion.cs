using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Response.Ssml;

public class AmazonEmotion : ICommonSsml
{
    public string Name { get; set; }

    public string Intensity { get; set; }

    public List<ISsml> Elements { get; set; } = [];

    public AmazonEmotion(string name, string intensity)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Intensity = intensity ?? throw new ArgumentNullException(nameof(intensity));
    }

    public AmazonEmotion(string name, string intensity, params ISsml[] elements)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Intensity = intensity ?? throw new ArgumentNullException(nameof(intensity));
        Elements = [.. elements];
    }

    public XNode ToXml()
    {
        return new XElement(Namespaces.TempAmazon + "emotion", new XAttribute("name", Name), new XAttribute("intensity", Intensity), Elements.Select(e => e.ToXml()));
    }
}