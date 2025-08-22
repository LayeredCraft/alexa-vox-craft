using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Response.Ssml;

public class Paragraph : ISsml
{
    public List<IParagraphSsml> Elements {get;set;} = [];

    public Paragraph() { }

    public Paragraph(params IParagraphSsml[] elements)
    {
        Elements = [.. elements];
    }

    public XNode ToXml()
    {
        return new XElement("p", Elements.Select(e => e.ToXml()));
    }
}