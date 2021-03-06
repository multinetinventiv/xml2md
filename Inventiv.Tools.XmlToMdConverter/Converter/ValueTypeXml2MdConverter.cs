﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.Converter
{
	public class ValueTypeXml2MdConverter : IXml2MdConverter
	{
		private readonly IDocumentProcessor documentProcessor;
		private readonly XmlManager xmlManager;

		public ValueTypeXml2MdConverter(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
			xmlManager = new XmlManager();
		}

		public virtual List<MdFile> Convert(string xmlContent)
		{
			var xmlDocument = xmlManager.GetXDocumentFromXmlContent(xmlContent);

			var documents = GetAssemblesWithDocuments(xmlDocument);
			if (documents == null || documents.Count == 0) return null;

			var mdContents = new List<MdFile>();

			foreach (var document in documents)
			{
				var node = document.Value.Root;

				var convertedMdContent = documentProcessor.ConvertDocumentToMarkDown(node);

				mdContents.Add(new MdFile(document.Key, convertedMdContent));
			}

			return mdContents;
		}

		private Dictionary<string, XDocument> GetAssemblesWithDocuments(XDocument xmlDocument)
		{

			var documents = new Dictionary<string, XDocument>();

			foreach (var member in xmlDocument.Elements("doc").Elements("members").Elements("member"))
			{
				var memberName = member.Attribute("name");
				if (memberName != null && memberName.Value.Contains("T:"))
				{
					var assembleyName = member.Attribute("name").Value.Split('.')[1];
					documents.Add(assembleyName, new XDocument(new XElement("doc",
						new XElement("assembly", new XElement("name", assembleyName)),
						new XElement("members", member)
					)));
					continue;
				}

				var document = documents.LastOrDefault();

				var members = document.Value.Elements("doc").Elements("members").FirstOrDefault();
				if (members != null) members.Add(member);
			}

			return documents;
		}
	}

}
