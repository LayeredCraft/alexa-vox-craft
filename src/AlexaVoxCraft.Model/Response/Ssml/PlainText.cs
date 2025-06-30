using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Response.Ssml;

public class PlainText:ICommonSsml
{
    public PlainText(string text)
    {
        Text = text;
    }

    public string Text { get; set; }

    public XNode ToXml()
    {
        return new XText(Text);
    }
}