using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class SeePage : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public SeePage(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x =>
			{
				var xx = documentProcessor.DocumentProcess("cref", x);
				return xx;
			};
		}

		public string Pattern { get { return "[{1}![](/assets/external-link-ltr-icon.png)]({0})"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			var value = elementValues.FirstOrDefault(v => v.Contains("!:"));
			if (!string.IsNullOrWhiteSpace(value))
			{
				elementValues[Array.IndexOf(elementValues, value)] = value.Replace("!:", string.Empty).Replace("(", "%28").Replace(")", "%29");
			}

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
