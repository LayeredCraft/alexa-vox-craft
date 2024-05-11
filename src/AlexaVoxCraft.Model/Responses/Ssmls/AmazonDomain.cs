﻿using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Responses.Ssmls;

public class AmazonDomain : ICommonSsml
{
    public string Name { get; set; }

    public List<ISsml> Elements { get; set; } = new List<ISsml>();

    public AmazonDomain(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public AmazonDomain(string name, params ISsml[] elements)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Elements = elements.ToList();
    }

    public XNode ToXml()
    {
        return new XElement(Namespaces.TempAmazon + "domain", new XAttribute("name", Name), Elements.Select(e => e.ToXml()));
    }
}