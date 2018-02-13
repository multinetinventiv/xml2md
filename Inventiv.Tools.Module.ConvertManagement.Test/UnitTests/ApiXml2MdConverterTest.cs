using System;
using System.Collections.Generic;
using Inventiv.Tools.XmlToMdConverter.Helpers;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using Inventiv.Tools.XmlToMdConverter.Converter;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;
using Moq;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.UnitTests
{
	[TestFixture]
	public class ApiXml2MdConverterTest : TagConverterTest<ApiXml2MdConverter>
	{

		#region SetUp & Helpers

		private IXml2MdConverter testing;
		private Mock<IConfiguration> mockConfiguration;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			mockConfiguration = new Mock<IConfiguration>();
			MockIncludeCaptionConfiguration(true);
			testing = new ApiXml2MdConverter(new DocumentProcessor(mockConfiguration.Object));
		}

		private void MockIncludeCaptionConfiguration(bool includeCaption)
		{
			mockConfiguration.Setup(c => c.IsIncludedCaption).Returns(includeCaption);
		}
		
		#endregion

		[Test]
		public void Xml_icerigi_parse_edilemez_ise_hata_atilir()
		{
			const string xmlContent = "<doc><members><member></members></member></doc>";

			Assert.Throws<FormatException>(() => testing.Convert(xmlContent));
		}

		[Test]
		public void Xml_iceriginde_dosya_olusturulmak_icin_gerekli_bilgiler_yoksa_bos_deger_doner()
		{
			//arrange
			var xmlContent = GenerateXml(string.Empty);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsNull(actual);
		}

		[Test]
		public void Xml_icerigindeki_method_tipindeki_memberlarin_her_bir_farkli_namespacesi_ayri_bir_md_dosyasidir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Debug(System.Object)"),
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Error(System.Object)"),
				GenerateMember("M:Multinet.Framework.Web.Mvc.Ui.HtmlExtensions.TimeRange()"),
				GenerateMember("M:Multinet.Framework.Web.Mvc.Ui.HtmlExtensions.TimeRange2()")
				);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("Logging", actual[0].FileName);
			Assert.AreEqual("Web", actual[1].FileName);
		}
		
		[Test]
		public void Ayni_namespacedeki_iki_methodun_md_formatindaki_basligi_ayni_dosya_icerisinde_sirayla_bulunur()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Debug(System.Object)"),
				GenerateMember("M:Multinet.Framework.Logging.ILogger.All(System.Object)")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.StartsWith("#### Method: `Debug(Object)`"));
			Assert.IsTrue(actual[0].Content.Contains("#### Method: `All(Object)`"));
		}

		[Test]
		public void Xml_icerigi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Debug(System.Object)")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.StartsWith("#### Method: `Debug(Object)`"));
		}

		[Test]
		public void Xml_icerigindeki_type_tipindeki_memberlar_md_formatina_cevrilmez()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Debug(System.Object)"),
				GenerateMember("F:Multinet.Framework.Logging.ILogger.LogLevel")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsFalse(actual[0].Content.Contains("#### Field: `LogLevel`\n\n\n\n---\n"));
		}
		
	}
}
