using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventiv.Tools.XmlToMdConverter.Helpers
{
	public static class ExtensionMethods
	{
		public static string FirstCharToUpper(this string input)
		{
			if (string.IsNullOrEmpty(input)) throw new ArgumentException("ARGH!");

			return string.Format("{0}{1}", input.First().ToString().Replace('i','ı').ToUpper(), string.Join("", input.Skip(1)));
		}
	}
}
