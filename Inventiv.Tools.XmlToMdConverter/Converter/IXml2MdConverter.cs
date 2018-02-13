using System.Collections.Generic;
using Inventiv.Tools.XmlToMdConverter.Helpers;

namespace Inventiv.Tools.XmlToMdConverter.Converter
{
	public interface IXml2MdConverter
	{
		List<MdFile> Convert(string xmlDocument);

	}
}
