using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Doc : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public Doc(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => new[]
			{
				x.Element("assembly").Element("name").Value,
				string.Join(string.Empty, x.Element("members").Elements("member").Select(documentProcessor.ConvertDocumentToMarkDown))
			};
		}

		public string Pattern { get { return "{0}\n\n"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			elementValues[1] = elementValues[1].Replace("T:System.", string.Empty);
			elementValues = new[] { elementValues[1] };

			return string.Format(Pattern, elementValues[0]);
		}
	}
}
