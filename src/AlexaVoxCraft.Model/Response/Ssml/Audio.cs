using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Response.Ssml;

public class Audio:ISsml
{
    public string Source { get; set; }
    public List<ISsml> Elements { get; set; } = [];

    public Audio() { }

    public Audio(params ISsml[] elements)
    {
        Elements = [.. elements];
    }

    public Audio(string source)
    {
        if(string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(source), "Source value required for Audio in Ssml");
        }

        Source = source;
    }

    public XNode ToXml()
    {
        return new XElement("audio", [new XAttribute("src",Source), .. Elements.Select(e => e.ToXml())]);
    }
}