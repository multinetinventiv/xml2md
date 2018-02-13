using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Param : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Param(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => documentProcessor.DocumentProcess("name", x);
		}

		public string Pattern { get { return "|**Parameter Name** | **Description** |\n|-----|------|\n|{0}|{1}|\n"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0],elementValues[1]);
		}
	}
}
