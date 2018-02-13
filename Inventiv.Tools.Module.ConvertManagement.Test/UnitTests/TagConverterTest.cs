using System;
using System.Text;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using Inventiv.Tools.XmlToMdConverter.Converter;
using Inventiv.Tools.XmlToMdConverter.DocumentProcessing;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.UnitTests
{
	public class TagConverterTest<TEntity> where TEntity : IXml2MdConverter
	{
		#region SetUp & Helpers


		private IXml2MdConverter testing;
		private IConfiguration configuration;
		
		public virtual void SetUp()
		{
			configuration = new Configuration();
			testing = Activator.CreateInstance(typeof(TEntity), new DocumentProcessor(configuration)) as IXml2MdConverter;
		}

		protected string GenerateXml(params string[] memberContents)
		{
			return "<?xml version=\"1.0\"?><doc><members>" +
				   string.Join(string.Empty, memberContents) +
				   "</members></doc>";
		}

		protected string GenerateMember(string memberName, params string[] memberContent)
		{
			return string.Format("<member name=\"{0}\">{1}</member>", memberName, string.Join(string.Empty, memberContent));
		}

		private string GenerateCode(string codeContent)
		{
			return string.Format("<code>{0}</code>", codeContent);
		}

		private string GenerateTypeInfo(string dbColumn, string typeInfoContent)
		{
			return string.Format("<typeInfo dbColumn=\"{0}\">{1}</typeInfo>", dbColumn, typeInfoContent);
		}

		private string GenerateExample(string exampleContent)
		{
			return string.Format("<example>{0}</example>", exampleContent);
		}

		private string GenerateException(string cref, string exceptionContent)
		{
			return string.Format("<exception cref=\"{0}\">{1}</exception>", cref, exceptionContent);
		}

		protected string GenerateReturns(string returnContent)
		{
			return string.Format("<returns>{0}</returns>", returnContent);
		}

		private string GenerateNone(string testContent)
		{
			return string.Format("<none>{0}</none>", testContent);
		}

		protected string GenerateModifier(string modifierContent)
		{
			return string.Format("<modifier>{0}</modifier>", modifierContent);
		}

		private string GenerateItalic(string italicContent)
		{
			return string.Format("<italic>{0}</italic>", italicContent);
		}

		private string GeneratePara(string paraContent)
		{
			return string.Format("<para>{0}</para>", paraContent);
		}

		private string GenerateParam(string parameterName, string paramContent)
		{
			return string.Format("<param name=\"{0}\">{1}</param>", parameterName, paramContent);
		}

		private string GenerateTypeParam(string parameterName, string paramContent)
		{
			return string.Format("<typeparam name=\"{0}\">{1}</typeparam>", parameterName, paramContent);
		}

		private string GenerateRemarks(string remarkContent)
		{
			return string.Format("<remarks>{0}</remarks>", remarkContent);
		}

		private string GenerateSeeAlso(string cref, string content)
		{
			return string.Format("<seealso cref=\"!:{0}\">{1}</seealso>", cref, content);
		}
		private string GenerateSeePage(string cref, string content)
		{
			return string.Format("<see cref=\"!:{0}\">{1}</see>", cref, content);
		}

		private string GenerateSeeAnchor(string cref, string content)
		{
			return string.Format("<see cref=\"!:#{0}\">{1}</see>", cref, content);
		}

		private string GenerateStrong(string content)
		{
			return string.Format("<strong>{0}</strong>", content);
		}

		private string GenerateSummary(string content)
		{
			return string.Format("<summary>{0}</summary>", content);
		}

		private string GenerateValue(string content)
		{
			return string.Format("<value>{0}</value>", content);
		}

		#endregion

		[Test]
		public void Code_tagi_md_formatina_cevrilir()
		{
			//arrange
			var codeContent = "string";
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateCode(codeContent)
				),
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateCode(codeContent)
					
			));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(string.Format("`{0}`", codeContent)));
		}

		[Test]
		public void TypeInfo_tagi_md_formatina_cevrilir()
		{
			//arrange
			const string typeInfoDbColumn = "dbColumn";
			const string typeInfoContent = "content";
			var testingContent = string.Format("|**Database Columns** | **Service Format** |\n|-----|------|\n|{0}|{1}|\n", typeInfoDbColumn, typeInfoContent);
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateTypeInfo(typeInfoDbColumn, typeInfoContent)
				),
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateTypeInfo(typeInfoDbColumn, typeInfoContent)
			));

			//act
			var actual = testing.Convert(xmlContent);
			
			//assert
			Assert.IsTrue(actual[0].Content.Contains(testingContent));
		}

		[Test]
		public void Example_tagi_md_formatina_cevrilir()
		{
			//arrange
			var exampleContent = "example content";
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateExample(exampleContent)
				),	
				GenerateMember("M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateExample(exampleContent)
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(string.Format("```csharp\n{0}\n```\n\n", exampleContent)));
		}

		[Test]
		public void Exception_tagi_md_formatina_cevrilir()
		{
			//arrange
			var cref = "T:System.ArgumentNullException";
			var exceptionContent = "exception content";
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateException(cref, exceptionContent)
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateException(cref, exceptionContent)
			));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(string.Format("**Error Type:** ArgumentNullException ({0})\n\n", exceptionContent)));
		}
		
		[Test]
		public void None_taginin_icerigi_md_formatina_cevrilmez()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateNone("test")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateNone("test")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsFalse(actual[0].Content.Contains("test"));
		}
		
		[Test]
		public void Italic_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateItalic("italicTest")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateItalic("italicTest")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("_italicTest_"));
		}

		[Test]
		public void Member_adi_M_ile_baslayan_member_method_olarak_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("M:System.Password.Parse")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### Method: `Parse()`\n\n\n\n---\n"));
		}

		[Test]
		public void Method_olarak_cevrilecek_memberin_adindaki_parametreler_md_formatinda_metod_adinin_sonunda_yer_alir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("M:System.Password.Parse(System.String)")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### Method: `Parse(String)`\n\n\n\n---\n"));
		}

		[Test]
		public void Method_memberinin_icindeki_modifier_taginin_icerigi_Method_adinin_onune_eklenir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("M:System.Password.Parse(System.String, System.String)",
					GenerateModifier("static")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### Method: `static Parse(String, String)`\n\n\n\n---\n"));
		}

		[Test]
		public void Method_memberinin_icindeki_modifier_ve_returns_taglerinin_icerikleri_Method_adinin_onune_eklenir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("M:System.Password.Parse(System.String, System.String)",
					GenerateModifier("static"),
					GenerateReturns("string")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### Method: `static string Parse(String, String)`\n\n\n\n---\n"));
		}

		[Test]
		public void Para_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GeneratePara("paraTest")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GeneratePara("paraTest")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("\nparaTest\n"));
		}

		[Test]
		public void Param_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateParam("parametreAdi", "aciklama")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateParam("parametreAdi", "aciklama")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("|**Parameter Name** | **Description** |\n|-----|------|\n|parametreAdi|aciklama|\n"));
		}

		[Test]
		public void Birden_cok_Param_tagi_pes_pese_gelirse_ikinci_ve_sonraki_paramlar_additionalparam_olarak_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password",
					GenerateParam("parametreAdi", "aciklama"),
					GenerateParam("parametreAdi1", "aciklama1"),
					GenerateParam("parametreAdi2", "aciklama2")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateParam("parametreAdi", "aciklama"),
					GenerateParam("parametreAdi1", "aciklama1"),
					GenerateParam("parametreAdi2", "aciklama2")
				));

			var testingBuilder = new StringBuilder();
			testingBuilder.Append("|**Parameter Name** | **Description** |\n|-----|------|\n|parametreAdi|aciklama|\n");
			testingBuilder.Append("|parametreAdi1|aciklama1|\n");
			testingBuilder.Append("|parametreAdi2|aciklama2|\n");

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(testingBuilder.ToString()));
		}


		[Test]
		public void Remarks_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateRemarks("remarkTest")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateRemarks("remarkTest")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("\n\n>remarkTest\n\n"));
		}

		[Test]
		public void SeeAlso_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateSeeAlso("http://google.com", "Google")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateSeeAlso("http://google.com", "Google")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("[Google![](/assets/external-link-ltr-icon.png)](http://google.com)"));
		}

		[Test]
		public void SeePage_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateSeePage("http://google.com", "Google")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateSeePage("http://google.com", "Google")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("[Google![](/assets/external-link-ltr-icon.png)](http://google.com)"));
		}

		[Test]
		public void SeeAnchor_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateSeeAnchor("mainpage.md", "MainPage")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateSeeAnchor("mainpage.md", "MainPage")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("[MainPage](!:#mainpage.md)"));
		}

		[Test]
		public void Strong_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateStrong("strongTest")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateStrong("strongTest")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("**strongTest**"));
		}

		[Test]
		public void Summary_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateSummary("summaryTest")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateSummary("summaryTest")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("summaryTest\n\n"));
		}

		[Test]
		public void TypeParam_tagi_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateTypeParam("parametreAdi", "aciklama")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateTypeParam("parametreAdi", "aciklama")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("|**Parameter Name** | **Description** |\n|-----|------|\n|parametreAdi|aciklama|\n"));
		}

		[Test]
		public void Value_tagi_md_formatina_convert_edilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember(
					"T:System.Password",
					GenerateValue("value")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateValue("value")
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("**Value:** value\n\n"));
		}

		[Test]
		public void Birden_cok_TypeParam_tagi_pes_pese_gelirse_ikinci_ve_sonraki_paramlar_additionalparam_olarak_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password",
					GenerateTypeParam("parametreAdi", "aciklama"),
					GenerateTypeParam("parametreAdi1", "aciklama1"),
					GenerateTypeParam("parametreAdi2", "aciklama2")
				),
				GenerateMember(
					"M:Multinet.Framework.Logging.ILogger.Debug(System.Object)",
					GenerateTypeParam("parametreAdi", "aciklama"),
					GenerateTypeParam("parametreAdi1", "aciklama1"),
					GenerateTypeParam("parametreAdi2", "aciklama2")
				));

			var testingBuilder = new StringBuilder();
			testingBuilder.Append("|**Parameter Name** | **Description** |\n|-----|------|\n|parametreAdi|aciklama|\n");
			testingBuilder.Append("|parametreAdi1|aciklama1|\n");
			testingBuilder.Append("|parametreAdi2|aciklama2|\n");

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains(testingBuilder.ToString()));
		}

		[Test]
		public void Ctor_ile_baslayan_member_adlari_constructor_olarak_md_formatina_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("M:System.Password.#ctor(System.Security.SecureString)"
				));

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### `Constructor(SecureString)`\n\n\n\n---\n"));
		}

		[Test]
		public void Member_adi_Extensions_iceriyorsa_ve_M_ile_basliyorsa_md_formatina_Extension_metod_olarak_cevrilir()
		{
			//arrange
			var xmlContent = GenerateXml(
				GenerateMember("T:System.Password"),
				GenerateMember("M:System.PasswordExtensions.ToPassword(System.String)")
			);

			//act
			var actual = testing.Convert(xmlContent);

			//assert
			Assert.IsTrue(actual[0].Content.Contains("#### Extension Method: `ToPassword(String)`\n\n\n\n---\n"));
		}

	}
}
