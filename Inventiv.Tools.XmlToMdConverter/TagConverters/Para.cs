using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Para : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Para(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => new[] { string.Join(string.Empty, x.Nodes().Select(documentProcessor.ConvertDocumentToMarkDown)) };
		}

		public string Pattern { get { return "\n{0}\n"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0]);
		}
	}
}
