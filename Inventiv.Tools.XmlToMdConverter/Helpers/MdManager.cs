using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Inventiv.Tools.XmlToMdConverter.Helpers
{
	internal static class MdManager
	{
		internal static string ToCodeBlock(this string content)
		{
			var lines = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var blank = lines[0].TakeWhile(x => x == ' ').Count() - 4;
			return string.Join("\n", lines.Select(x => new string(x.SkipWhile((y, i) => i < blank).ToArray())));
		}

		internal static string[] GenerateCaption(string elementName, string[] elementValues)
		{
			var modifierText = string.Empty;
			var elementValueSplited = elementValues[1].Split(new[] {"||"}, StringSplitOptions.None);
			if(elementValueSplited.Length > 2 && !elementValueSplited[1].Contains("|"))
			{
				var firstIndex = elementValues[1].IndexOf("||", StringComparison.Ordinal) + 2;
				var lastIndex = elementValues[1].LastIndexOf("||", StringComparison.Ordinal);
				modifierText = elementValues[1].Substring(firstIndex, lastIndex - firstIndex);
			}

			var returnList = elementValues[1].Split(new[] { "$$" }, StringSplitOptions.None);
			var returnText = returnList.Length > 2 ? returnList[1] : string.Empty;
			
			var builder = new StringBuilder();
			builder.AppendFormat("{0}: `", elementName.FirstCharToUpper());
			if (!string.IsNullOrWhiteSpace(modifierText)) builder.AppendFormat("{0} ", modifierText);
			if (!string.IsNullOrWhiteSpace(returnText)) builder.AppendFormat("{0} ", returnText);
			builder.AppendFormat("{0}`", GetCaptionNameWithParameters(elementValues[0]));

			elementValues[0] = builder.ToString();

			elementValues[1] = elementValues[1].Replace(string.Format("$${0}$$", returnText), string.Empty);
			elementValues[1] = elementValues[1].Replace(string.Format("||{0}||", modifierText), string.Empty);

			return elementValues;
		}

		private static string GetCaptionNameWithParameters(string caption)
		{
			var splittedNameAndParamters = caption.Split('(');

			var methodName = splittedNameAndParamters[0].Split('.').LastOrDefault();

			if (splittedNameAndParamters.Length == 1) return methodName;

			if (splittedNameAndParamters[1] != ")")
			{
				var parameters = splittedNameAndParamters[1].Split(',');
				
				parameters = parameters.Select(param => param.Split('.').LastOrDefault()).ToArray();

				splittedNameAndParamters[1] = string.Join(", ", parameters);
			}

			return string.Format("{0}({1}", methodName, splittedNameAndParamters[1]);
		}

	}
}