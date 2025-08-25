using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Response.Ssml;

public class Voice:ICommonSsml
{
    public string Name { get; set; }

    public List<ISsml> Elements { get; set; } = [];

    public Voice(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Voice(string name, params ISsml[] elements)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Elements = [.. elements];
    }

    public XNode ToXml()
    {
        return new XElement("voice", new XAttribute("name",Name), Elements.Select(e => e.ToXml()));
    }
}