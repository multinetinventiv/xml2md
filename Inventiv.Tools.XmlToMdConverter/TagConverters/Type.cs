using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Type : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Type(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => documentProcessor.DocumentProcess("name", x);
		}

		public string Pattern
		{
			get
			{
				return !documentProcessor.Configuration.IsIncludedCaption ? string.Empty : "# {0}\n\n{1}\n\n---\n";
			}
		}

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
