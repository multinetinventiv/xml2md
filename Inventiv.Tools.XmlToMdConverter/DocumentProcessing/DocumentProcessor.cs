using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.TagConverters;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using Type = System.Type;

namespace Inventiv.Tools.XmlToMdConverter.DocumentProcessing
{
	public class DocumentProcessor : IDocumentProcessor
	{
		public IConfiguration Configuration { get; private set; }

		public DocumentProcessor(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public Func<string, XElement, string[]> DocumentProcess
		{
			get
			{
				return (att, node) => new[]
				{
					node.Attribute(att).Value,
					string.Join(string.Empty, node.Nodes().Select(n => ConvertDocumentToMarkDown(n)))
				};
			}
		}

		public string ConvertDocumentToMarkDown(XNode node)
		{

			switch (node.NodeType)
			{
				case XmlNodeType.Element:
					return ConvertElementToMarkDown(node);
				case XmlNodeType.Text:
					return ConvertTextToMarkdown(node);
			}

			return string.Empty;
		}

		private string ConvertTextToMarkdown(XNode node)
		{
			var value = ((XText)node).Value;
			return Regex.Replace(value.Replace('\n', ' '), @"\s+", " "); ;
		}

		private string ConvertElementToMarkDown(XNode node)
		{
			var element = (XElement)node;

			var tagInstance = GetTagInstance(element);

			if (tagInstance == null) throw new InvalidCastException(string.Format("{0} tag'i cast edilemedi", element.Name.LocalName));

			return tagInstance.GetConvertedContent(element);

		}

		private ITagConverter GetTagInstance(XElement element)
		{
			var elementName = element.Name.LocalName;

			switch (elementName)
			{
				case "member":
					return GetTagInstanceByAttributeName(element);

				case "typeparam":
					if (element.PreviousNode != null && element.PreviousNode.ToString().StartsWith("<typeparam"))
					{
						return new AdditionalParam(new Param(this));
					}

					return new Param(this);

				case "see":
				case "seealso":
					elementName = element.Attribute("cref").Value.StartsWith("!:#") ? "SeeAnchor" : "SeePage";
					break;

				case "param":
					if (element.PreviousNode != null && element.PreviousNode.ToString().StartsWith("<param"))
					{
						return new AdditionalParam(new Param(this));
					}
					break;
			}

			var type = Type.GetType(string.Format("Inventiv.Framework.XmlToMdConverter.TagConverters.{0}", elementName.FirstCharToUpper()));
			if (type == null) return null;

			switch (elementName)
			{
				case "example":
				case "none":
					return Activator.CreateInstance(type) as ITagConverter;
			}

			return Activator.CreateInstance(type, this) as ITagConverter;
		}

		private ITagConverter GetTagInstanceByAttributeName(XElement element)
		{
			switch (element.Attribute("name").Value[0])
			{
				case 'T':
					return new TagConverters.Type(this);

				case 'F':
					return new Field(this);

				case 'M':
					return new Method(this);

				case 'P':
					return new Property(this);
				default:
					return null;
			}

		}
	}
}
