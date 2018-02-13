using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Property : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Property(IDocumentProcessor documentProcessor)
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

			elementValues = MdManager.GenerateCaption("Property", elementValues);

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
