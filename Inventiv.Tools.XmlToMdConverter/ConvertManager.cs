using System;
using System.IO;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.Converter;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;
using Inventiv.Tools.XmlToMdConverter.FileSystem;

namespace Inventiv.Tools.XmlToMdConverter
{
	public class ConvertManager
	{
		private readonly IFileSystem fileSystem;
		private readonly IXml2MdConverter xml2MdConverter;
		private readonly IDocumentProcessor documentProcessor;

		public ConvertManager(IXml2MdConverter xml2MdConverter, IFileSystem fileSystem, IDocumentProcessor documentProcessor)
		{
			this.xml2MdConverter = xml2MdConverter;
			this.fileSystem = fileSystem;
			this.documentProcessor = documentProcessor;
		}

		public void Convert(string sourceFilePath, string targetFolderPath)
		{
			if (string.IsNullOrWhiteSpace(sourceFilePath)) throw new ArgumentNullException(sourceFilePath);
			if (Path.GetExtension(sourceFilePath) != ".xml") throw new FormatException();
			if (string.IsNullOrWhiteSpace(targetFolderPath)) throw new ArgumentNullException(targetFolderPath);

			var xmlContent = fileSystem.Read(sourceFilePath);
			if (string.IsNullOrWhiteSpace(xmlContent))
			{
				fileSystem.RemoveAll(targetFolderPath);
				return;
			}

			var mdFilesInformation = xml2MdConverter.Convert(xmlContent);
			if (mdFilesInformation == null || mdFilesInformation.Count == 0)
			{
				fileSystem.RemoveAll(targetFolderPath);
				return;
			}

			foreach (var mdFile in mdFilesInformation)
			{
				var targetFilePath = string.Format("{0}\\{1}.md", targetFolderPath, mdFile.FileName);

				ExportMdFiles(mdFile, targetFilePath);
			}

		}

		private void ExportMdFiles(MdFile mdFile, string targetFilePath)
		{
			if (mdFile.IsProtectIntroduction)
			{
				string introductionContent;
				if (!fileSystem.Exists(targetFilePath))
				{
					fileSystem.Write(targetFilePath, string.Empty);
					
					//TODO Test edilemedi
					var caption = Path.GetFileNameWithoutExtension(targetFilePath);
					introductionContent = string.Format(new TagConverters.Type(documentProcessor).Pattern, caption, string.Empty);
				}
				else
				{
					var content = fileSystem.Read(targetFilePath);
					introductionContent = content.Contains("#### Method:")
						? content.Split(new[] { "#### Method:" }, StringSplitOptions.None)[0]
						: content;
				}

				fileSystem.Write(targetFilePath, string.Format("{0}{1}", introductionContent, mdFile.Content));
			}
			else
			{
				fileSystem.Write(targetFilePath, mdFile.Content);
			}
		}
	}
}

