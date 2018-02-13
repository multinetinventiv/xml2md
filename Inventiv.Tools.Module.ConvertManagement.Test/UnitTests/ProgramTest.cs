using System.Collections.Generic;
using System.IO;
using System.Linq;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using Inventiv.Tools.XmlToMdConverter.Converter;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;
using Inventiv.Tools.XmlToMdConverter.FileSystem;
using Moq;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.UnitTests
{
	[TestFixture]
	public class ProgramTest
	{
		#region SetUp & Helpers

		private const string xmlDocumentPath = "//Multinet.System.xml";

		private Mock<IFileSystem> mockFileSystem;
		private Mock<IConfiguration> mockConfiguration;
		private IDocumentProcessor documentProcessor;

		private string RootPath
		{
			get
			{
				return Path.Combine(Path.GetTempPath(), "FileSystemTest");
			}
		}

		[SetUp]
		public void SetUp()
		{
			mockFileSystem = new Mock<IFileSystem>();
			mockConfiguration = new Mock<IConfiguration>();
			documentProcessor = new DocumentProcessor(mockConfiguration.Object);
		}

		private void MockFileSystemRead(string xmlContent, string path = xmlDocumentPath)
		{
			mockFileSystem.Setup(fs => fs.Read(path)).Returns(xmlContent);
		}

		private static Mock<IXml2MdConverter> MockXml2MdConverter(MdFile mdFile)
		{
			var mockXml2MdConverter = new Mock<IXml2MdConverter>();

			mockXml2MdConverter.Setup(x2m => x2m.Convert(It.IsAny<string>())).Returns(() => new List<MdFile>() { mdFile });
			return mockXml2MdConverter;
		}

		private Program CreateProgramInstance(params KeyValuePair<string, IXml2MdConverter>[] programInformation)
		{
			var converters = new Dictionary<string, ConverterInformation>();
			if (programInformation == null || programInformation.Length == 0)
			{
				var mockXml2MdConverter = new Mock<IXml2MdConverter>();
				converters.Add("testKey", new ConverterInformation(mockXml2MdConverter.Object, "desc"));

			}
			else
			{
				programInformation.ToList().ForEach(pi => { converters.Add(pi.Key, new ConverterInformation(pi.Value, "desc")); });
			}

			return new Program(mockFileSystem.Object, converters, documentProcessor, documentProcessor.Configuration);

		}

		private void MockFileSystemExist(string path, bool isExist)
		{
			mockFileSystem.Setup(fs => fs.Exists(path)).Returns(isExist);
		}

		private void MockIncludeCaptionConfiguration(bool includeCaption)
		{
			mockConfiguration.Setup(c => c.IsIncludedCaption).Returns(includeCaption);
		}

		#endregion

		[Test]
		public void Ilk_parametre_value_type_olarak_gonderilirse_ValueTypeXml2MdConvertera_yonlendirme_yapilir()
		{
			//arrange
			var mockValueTypeXml2MdConverter = new Mock<ValueTypeXml2MdConverter>(documentProcessor);
			var testing = CreateProgramInstance(new KeyValuePair<string, IXml2MdConverter>("value-type", mockValueTypeXml2MdConverter.Object));

			MockFileSystemRead("<?xml version=\"1.0\"?><doc><members></members></doc>");

			//act
			testing.Execute("value-type", xmlDocumentPath, RootPath);

			//assert
			mockValueTypeXml2MdConverter.Verify(vt => vt.Convert(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Ilk_parametre_api_olarak_gonderilirse_ApiXml2MdConvertera_yonlendirme_yapilir()
		{
			//arrange
			var mockApiXml2MdConverter = new Mock<ApiXml2MdConverter>(documentProcessor);
			var testing = CreateProgramInstance(new KeyValuePair<string, IXml2MdConverter>("api", mockApiXml2MdConverter.Object));

			MockFileSystemRead("valueTypeTest");

			//act
			testing.Execute("api", xmlDocumentPath, RootPath);

			//assert
			mockApiXml2MdConverter.Verify(api => api.Convert(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Ilk_parametre_desteklenmeyen_bir_key_ise_help_gosterilir_ve_hata_doner()
		{
			//arrange
			var testing = CreateProgramInstance(
				new KeyValuePair<string, IXml2MdConverter>("api", null),
				new KeyValuePair<string, IXml2MdConverter>("value-type", null)
				);

			//act
			var actual = testing.Execute("test", xmlDocumentPath, RootPath);

			//assert
			Assert.AreEqual(1, actual);
		}

		[Test]
		public void Kaynak_dosyasinin_adresi_bos_ise_hata_atilir()
		{
			var testing = CreateProgramInstance(null);

			var actual = testing.Execute("testKey");

			Assert.AreEqual(1, actual);
		}

		[Test]
		public void Kaynak_dosyasinin_uzantisi_xml_degil_ise_hata_atilir()
		{
			var testing = CreateProgramInstance(null);

			var actual = testing.Execute("testKey", "//testing.txt", RootPath);

			Assert.AreEqual(1, actual);
		}

		[Test]
		public void Hedef_dosyasinin_adresi_bos_ise_hata_atilir()
		{
			var testing = CreateProgramInstance(null);

			var actual = testing.Execute("testKey", xmlDocumentPath);

			Assert.AreEqual(1, actual);
		}

		[Test]
		public void Xml_icerigi_bos_ise_hedef_dosyalar_silinir()
		{
			//arrange
			var testing = CreateProgramInstance(null);
			MockFileSystemRead(string.Empty);

			//act
			var actual = testing.Execute("testKey", xmlDocumentPath, RootPath);

			//assert
			mockFileSystem.Verify(fs => fs.RemoveAll(It.IsAny<string>()), Times.Once);
			Assert.AreEqual(0, actual);
		}

		[Test]
		public void Xml_iceriginde_md_dosyasi_olusturmak_icin_gerekli_bilgiler_yoksa_hedef_klasor_altindaki_dosyalar_silinir()
		{
			//arrange
			var testing = CreateProgramInstance(null);
			MockFileSystemRead(string.Empty);

			//act
			var actual = testing.Execute("testKey", xmlDocumentPath, RootPath);

			//assert
			mockFileSystem.Verify(fs => fs.RemoveAll(It.IsAny<string>()), Times.Once);
			Assert.AreEqual(0, actual);
		}

		[Test]
		public void Md_formatindaki_icerik_hedef_dosyaya_yazilir()
		{
			//arrange
			var fileName = "fileName";
			var content = "content";
			var targetMdPath = string.Format("{0}\\{1}.md", RootPath, fileName);
			var mockXml2MdConverter = MockXml2MdConverter(new MdFile(fileName, content));
			var testing = CreateProgramInstance(new KeyValuePair<string, IXml2MdConverter>("testKey", mockXml2MdConverter.Object));

			MockFileSystemRead(content);

			//act
			testing.Execute("testKey", xmlDocumentPath, RootPath);

			//assert
			mockFileSystem.Verify(fs => fs.Write(targetMdPath, content), Times.Once);
		}

		[Test]
		public void Md_formatindaki_icerik_hedef_dosyaya_yazilirken_hedef_dosyadaki_giris_icerigine_eklenebilir()
		{
			//arrange
			var fileName = "fileName";
			var content = "content---";
			var targetMdPath = string.Format("{0}\\{1}.md", RootPath, fileName);
			var mockXml2MdConverter = MockXml2MdConverter(new MdFile(fileName, content, true));
			var testing = CreateProgramInstance(new KeyValuePair<string, IXml2MdConverter>("testKey", mockXml2MdConverter.Object));

			MockFileSystemRead(content);
			MockFileSystemRead(content, targetMdPath);
			MockFileSystemExist(targetMdPath, true);

			//act
			testing.Execute("testKey", xmlDocumentPath, RootPath);

			//assert
			mockFileSystem.Verify(fs => fs.Read(targetMdPath), Times.Once);
			mockFileSystem.Verify(fs => fs.Write(targetMdPath, string.Format("{0}{0}", content)), Times.Once);
		}

		[Test]
		public void Md_formatindaki_icerik_hedef_dosyaya_yazilirken_hedef_dosya_yoksa_dosya_olusturur_ve_dosya_adi_ile_giris_icerigi_olusturur()
		{
			//arrange
			var fileName = "fileName";
			var content = "content---";
			var targetMdPath = string.Format("{0}\\{1}.md", RootPath, fileName);
			var mockXml2MdConverter = MockXml2MdConverter(new MdFile(fileName, content, true));
			var testing = CreateProgramInstance(new KeyValuePair<string, IXml2MdConverter>("testKey", mockXml2MdConverter.Object));

			MockFileSystemRead(content);
			MockFileSystemRead(content, targetMdPath);
			MockFileSystemExist(targetMdPath, false);
			MockIncludeCaptionConfiguration(true);

			//act
			testing.Execute("testKey", xmlDocumentPath, RootPath);

			//assert
			mockFileSystem.Verify(fs => fs.Write(targetMdPath, string.Format("# {0}\n\n\n\n---\n{1}", fileName, content)), Times.Once);

		}

		[Test]
		public void Hedef_dosyasinin_basliga_sahip_olmasi_parametriktir()
		{
			//arrange
			var mockXml2MdConverter = MockXml2MdConverter(null);
			var testing = CreateProgramInstance(new KeyValuePair<string, IXml2MdConverter>("testKey", mockXml2MdConverter.Object));

			testing.Execute("testKey", xmlDocumentPath, RootPath, "false");
			mockConfiguration.Verify(c=>c.SetConfiguration(false),Times.Once);

			testing = CreateProgramInstance(new KeyValuePair<string, IXml2MdConverter>("testKey", mockXml2MdConverter.Object));
			
			testing.Execute("testKey", xmlDocumentPath, RootPath, "true");
			mockConfiguration.Verify(c=>c.SetConfiguration(true),Times.Once);
		}
	}
}
