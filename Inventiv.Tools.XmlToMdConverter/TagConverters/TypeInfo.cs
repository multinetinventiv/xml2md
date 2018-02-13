using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class TypeInfo : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public TypeInfo(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => documentProcessor.DocumentProcess("dbColumn", x);
		}

		public string Pattern
		{
			get
			{
				return "|**Database Columns** | **Service Format** |\n|-----|------|\n|{0}|{1}|\n";
			}
		}

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
