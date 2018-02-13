using System;
using System.Collections.Generic;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using Inventiv.Tools.XmlToMdConverter.Converter;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;
using Inventiv.Tools.XmlToMdConverter.FileSystem;
using Microsoft.Extensions.CommandLineUtils;

namespace Inventiv.Tools.XmlToMdConverter
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var fileSystem = new Tools.XmlToMdConverter.FileSystem.FileSystem();
			var configuration = new Configuration();
			var documentProcessor = new DocumentProcessor(configuration);

			new Program(
				fileSystem,
				new Dictionary<string, ConverterInformation>
				{
					{"value-type", 
						new ConverterInformation(
						 	new ValueTypeXml2MdConverter(documentProcessor), 
							"Value Type'ların xml dökümanlarını Markdown formatına çevirip belirtilen klasör altına export eder.")
					}, 
					{"api", 
						new ConverterInformation(
							new ApiXml2MdConverter(documentProcessor), 
							"API'ların xml dökümanlarını Markdown formatına çevirip belirtilen klasör altına export eder."
							)}
				},
				documentProcessor,
				configuration
				).Execute(args);
		}

		private readonly IFileSystem fileSystem;
		private readonly Dictionary<string, ConverterInformation> converters;
		private readonly IDocumentProcessor documentProcessor;
		private readonly IConfiguration configuration;

		private readonly CommandLineApplication app;

		public Program(IFileSystem fileSystem, Dictionary<string, ConverterInformation> converters, IDocumentProcessor documentProcessor, IConfiguration configuration)
		{
			this.fileSystem = fileSystem;
			this.converters = converters;
			this.documentProcessor = documentProcessor;
			this.configuration = configuration;

			app = new CommandLineApplication();
			foreach (var convertersKey in converters.Keys)
			{
				var key = convertersKey;
				app.Command(convertersKey, config =>
				{
					config.Description = this.converters[key].Description;
					config.HelpOption("-? | -h | --help");

					var sourceXmlPath = config.Argument("sourceXmlPath", "Xml dökümanının bulunduğu path");
					var targetDirectoryPath = config.Argument("targetDirectoryPath", "Markdown dosyalarının export edileceği klasörün path'i");
					var includeCaption = config.Argument("includeCaption", "Md dosyalarına başlık ekleme opsiyonu");

					config.OnExecute(() =>
					{
						try
						{
							if (includeCaption.Value != null) { this.configuration.SetConfiguration(bool.Parse(includeCaption.Value)); }

							var manager = new ConvertManager(this.converters[key].Converter, this.fileSystem, this.documentProcessor);
							manager.Convert(sourceXmlPath.Value, targetDirectoryPath.Value);

							return 0;
						}
						catch (Exception)
						{
							app.ShowHelp();
							return 1;
						}
					});
				});
			}

			app.HelpOption("-? | -h | --help");
		}

		public int Execute(params string[] args)
		{
			if (args.Length == 0)
			{
				app.ShowHelp();
				return 1;
			}

			try
			{
				return app.Execute(args);
			}
			catch (Exception ex)
			{
				app.ShowHelp();
				return 1;
			}

		}
	}
}
