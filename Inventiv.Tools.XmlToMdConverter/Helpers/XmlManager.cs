using System;
using System.Xml.Linq;

namespace Inventiv.Tools.XmlToMdConverter.Helpers
{

	internal class XmlManager
	{
		internal XDocument GetXDocumentFromXmlContent(string content)
		{
			try
			{
				return XDocument.Parse(content);
			}
			catch (Exception)
			{
				throw new FormatException(string.Format("Xml icerigi parse edilemedi. Icerik: {0}", content));
			}
		}

	}
}