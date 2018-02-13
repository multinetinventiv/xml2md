using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class None : ITagConverter
	{
		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x => new string[0];
		}

		public string Pattern { get { return string.Empty; } }

		public string GetConvertedContent(XElement element)
		{
			return string.Empty;
		}
	}
}
