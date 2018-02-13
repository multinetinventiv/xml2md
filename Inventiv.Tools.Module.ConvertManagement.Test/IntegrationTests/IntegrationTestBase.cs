using System.Collections.Generic;
using System.IO;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using Inventiv.Tools.XmlToMdConverter.Converter;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;
using Inventiv.Tools.XmlToMdConverter.FileSystem;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.IntegrationTests
{
	public class IntegrationTestBase
	{
		protected readonly string targetFolderPath = string.Format(@"{0}\IntegrationTests\ExportedMdFiles", TestContext.CurrentContext.TestDirectory);
		protected IFileSystem fileSystem;

		protected Program testing;

		protected virtual void SetUp()
		{
			fileSystem = new Tools.XmlToMdConverter.FileSystem.FileSystem();
			var configuration = new Configuration();
			var documentProcessor = new DocumentProcessor(configuration);

			testing = new Program(
					fileSystem,
					new Dictionary<string, ConverterInformation>
				{
					{
						"value-type",
						new ConverterInformation(
							new ValueTypeXml2MdConverter(documentProcessor),
							"Value Type'ların xml dökümanlarını Markdown formatına çevirip belirtilen klasör altına export eder.")
					},
					{
						"api",
						new ConverterInformation(
							new ApiXml2MdConverter(documentProcessor),
							"API'ların xml dökümanlarını Markdown formatına çevirip belirtilen klasör altına export eder."
						)
					}
				},
				documentProcessor,
				configuration);

			Directory.CreateDirectory(targetFolderPath);
		}

		public virtual void TearDown()
		{
			fileSystem.RemoveAll(targetFolderPath);
		}
	}
}
