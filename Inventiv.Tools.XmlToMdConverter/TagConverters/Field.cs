using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Field : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Field(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => documentProcessor.DocumentProcess("name", x);
		}

		public string Pattern { get { return "#### {0}\n\n{1}\n\n---\n"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			var rootNode = element.Nodes().FirstOrDefault();
			if (rootNode != null && rootNode.ToString().StartsWith("<none>")) return string.Empty;

			elementValues = MdManager.GenerateCaption("Field", elementValues);

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
