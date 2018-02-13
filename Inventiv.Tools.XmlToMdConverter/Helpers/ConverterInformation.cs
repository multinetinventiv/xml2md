using Inventiv.Tools.XmlToMdConverter.Converter;

namespace Inventiv.Tools.XmlToMdConverter.Helpers
{
	public class ConverterInformation
	{
		public ConverterInformation(IXml2MdConverter converter, string description)
		{
			Converter = converter;
			Description = description;
		}

		public IXml2MdConverter Converter { get; private set; }
		public string Description { get; private set; }
	}
}
