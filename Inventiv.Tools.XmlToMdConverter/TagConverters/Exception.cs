using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Exception : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Exception(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => documentProcessor.DocumentProcess("cref", x);
		}

		public string Pattern { get { return "**Error Type:** {0} ({1})\n\n"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
