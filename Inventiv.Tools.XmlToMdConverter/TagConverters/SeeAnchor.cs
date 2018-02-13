﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;

namespace Inventiv.Tools.XmlToMdConverter.TagConverters
{
	internal class SeeAnchor : ITagConverter
	{
		private readonly IDocumentProcessor documentProcessor;

		public SeeAnchor(IDocumentProcessor documentProcessor)
		{
			this.documentProcessor = documentProcessor;
		}

		public Func<XElement, IEnumerable<string>> GetParseFunction()
		{
			return x =>
			{
				var xx = documentProcessor.DocumentProcess("cref", x);
				return xx;
			};
		}

		public string Pattern { get { return "[{1}]({0})"; } }

		public string GetConvertedContent(XElement element)
		{
			var elementValues = GetParseFunction()(element).ToArray();

			return string.Format(Pattern, elementValues[0], elementValues[1]);
		}
	}
}
