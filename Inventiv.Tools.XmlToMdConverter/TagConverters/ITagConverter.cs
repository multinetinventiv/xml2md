using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	public interface ITagConverter
	{
		Func<XElement, IEnumerable<string>> GetParseFunction();
		string Pattern { get; }

		string GetConvertedContent(XElement element);

	}
}
