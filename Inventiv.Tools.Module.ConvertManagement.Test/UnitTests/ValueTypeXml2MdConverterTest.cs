using System;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using Inventiv.Tools.XmlToMdConverter.Converter;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;
using Moq;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.UnitTests
{
	[TestFixture]
	public class ValueTypeXml2MdConverterTest : TagConverterTest<ValueTypeXml2MdConverter>
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
			testing = new ValueTypeXml2MdConverter(new DocumentProcessor(mockConfiguration.Object));
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
		public void Xml_iceriginde_type_tipindeki_memberlar_kadar_md_dosyasi_olusturulur()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("T:System.DateRange"),
				GenerateMember("T:System.Iban")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.AreEqual(3, actual.Count);
		}

		[Test]
		public void Xml_icerigindeki_type_tipindeki_memberlarin_adlarinin_son_kelimesi_dosya_adlaridir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("T:System.DateRange"),
				GenerateMember("T:System.Iban")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.AreEqual("Password", actual[0].FileName);
			Assert.AreEqual("DateRange", actual[1].FileName);
			Assert.AreEqual("Iban", actual[2].FileName);
		}

		[Test]
		public void Member_adi_F_ile_baslayan_member_field_olarak__contentindeki_Returns_icerigide_deger_tipi_olarak_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("F:System.Password.Default")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(string.Format("#### Field: `Default`\n\n{0}\n\n---\n", string.Empty)));
		}

		[Test]
		public void Field_memberinin_icindeki_returns_taginin_icerigi_Field_adinin_onune_eklenir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("F:System.Password.Default",
					GenerateReturns("string")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(string.Format("#### Field: `string Default`\n\n{0}\n\n---\n", string.Empty)));
		}

		[Test]
		public void Field_memberinin_icindeki_modifier_taginin_icerigi_Field_adinin_onune_eklenir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("F:System.Password.Default",
					GenerateModifier("static")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### Field: `static Default`\n\n\n\n---\n"));
		}

		[Test]
		public void Field_memberinin_icindeki_modifier_ve_returns_taglerinin_icerikleri_Field_adinin_onune_eklenir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("F:System.Password.Default",
					GenerateModifier("static"),
					GenerateReturns("string")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(string.Format("#### Field: `static string Default`\n\n{0}\n\n---\n", string.Empty)));
		}
		
		[Test]
		public void Member_adi_P_ile_baslayan_member_property_olarak_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("P:System.Password.Value")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### Property: `Value`\n\n\n\n---\n"));
		}

		[Test]
		public void Modifier_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateModifier("static readonly")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("||static readonly||"));
		}

		[Test]
		public void Returns_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateReturns("string")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("$$string$$"));
		}

		[Test]
		public void Member_adi_T_ile_baslayan_memberin_adi__baslik_konfigurasyonu_true_ise_md_iceriginin_basligi_olur()
		{
			//arrange
			MockIncludeCaptionConfiguration(true);
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.StartsWith("# Password"));
		}

		[Test]
		public void Member_adi_T_ile_baslayan_memberin_adi__baslik_konfigurasyonu_false_ise_md_icerigi_basliksizdir()
		{
			//arrange
			MockIncludeCaptionConfiguration(false);
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsFalse(actual[0].Content.StartsWith("# Password"));
		}

	}
}
