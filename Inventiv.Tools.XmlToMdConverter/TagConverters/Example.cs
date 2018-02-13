using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class Example : ITagConverter
	{

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => new[] { x.Value.ToCodeBlock() };
		}

		public string Pattern { get { return "```csharp\n{0}\n```\n\n"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0]);
		}
	}
}
