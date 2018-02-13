using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventiv.Tools.XmlToMdConverter.Configurations;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.UnitTests
{
	[TestFixture]
	public class ConfigurationTest
	{
		private IConfiguration testing;

		[SetUp]
		public void SetUp()
		{
			testing = new Configuration();
		}

		[Test]
		public void Baslik_icerme_konfigurasyonu_belirtilmemis_ise_varsayilan_olarak_baslik_icerme_aktiftir()
		{
			Assert.IsTrue(testing.IsIncludedCaption);
		}

		[Test]
		public void Baslik_Icerme_islemi_konfigure_edilebilir()
		{
			testing.SetConfiguration(true);
			Assert.IsTrue(testing.IsIncludedCaption);


			testing.SetConfiguration(false);
			Assert.IsFalse(testing.IsIncludedCaption);
		}
	}
}
