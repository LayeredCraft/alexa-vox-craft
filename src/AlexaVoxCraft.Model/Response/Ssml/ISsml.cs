using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Response.Ssml;

public interface ISsml
{
    XNode ToXml();
}