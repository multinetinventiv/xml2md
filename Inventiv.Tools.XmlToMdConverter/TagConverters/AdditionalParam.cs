using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class AdditionalParam : ITagConverter
	{
		private readonly ITagConverter paramTagConverter;

		public AdditionalParam(ITagConverter paramTagConverter)
		{
			this.paramTagConverter = paramTagConverter;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			throw new NotSupportedException();
		}

		public string Pattern { get { return "|{0}|{1}|\n"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = paramTagConverter.GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
