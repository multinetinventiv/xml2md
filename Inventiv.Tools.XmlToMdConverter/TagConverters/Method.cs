using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Method : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Method(IDocumentProcessor documentProcessor)
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

			elementValues[0] += !elementValues[0].Contains("(") ? "()" : string.Empty;
			var isExtension = elementValues[0].Contains("Extension");
			elementValues = MdManager.GenerateCaption("Method", elementValues);

			if (elementValues[0].Contains("#ctor"))
			{
				elementValues[0] = elementValues[0].Replace("#ctor", "Constructor").Replace("Method: ", string.Empty);
			}

			elementValues[0] = isExtension ? string.Format("Extension {0}", elementValues[0]) : elementValues[0];

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
