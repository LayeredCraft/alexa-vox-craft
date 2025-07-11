﻿using System.Xml.Linq;

namespace AlexaVoxCraft.Model.Response.Ssml;

public class Speech
{

	public Speech()
	{
	}

	public Speech(params ISsml[] elements)
	{
		Elements = elements.ToList();
	}

	public List<ISsml> Elements { get; set; } = new List<ISsml>();


	public string ToXml()
	{
		if (Elements.Count == 0)
		{
			throw new InvalidOperationException("No text available");
		}

		XElement root = new XElement("speak", 
			new XAttribute(XNamespace.Xmlns + "amazon", Namespaces.TempAmazon),
			new XAttribute(XNamespace.Xmlns + "alexa",Namespaces.TempAlexa));
		root.Add(Elements.Select(e => e.ToXml()));

		string xmlString = root.ToString(SaveOptions.DisableFormatting);

		if (xmlString.StartsWith("<speak>", StringComparison.Ordinal))
		{
			return xmlString;
		}

		var endOfSpeakTag = xmlString.IndexOf('>');
		return "<speak>" + xmlString.Substring(endOfSpeakTag + 1);
	}
}