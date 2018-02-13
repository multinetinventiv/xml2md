using System;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Configurations;

namespace Inventiv.Tools.XmlToMdConverter.DocumentProcessing
{
	public interface IDocumentProcessor
	{
		IConfiguration Configuration { get; }

		Func<string, XElement, string[]> DocumentProcess { get; }

		string ConvertDocumentToMarkDown(XNode node);
	}
}
