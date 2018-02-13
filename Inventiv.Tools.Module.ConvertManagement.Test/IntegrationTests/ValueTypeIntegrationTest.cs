using System.IO;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.IntegrationTests
{
	[TestFixture]
	public class ValueTypeIntegrationTest : IntegrationTestBase
	{
		#region SetUp & Helpers

		protected readonly string xmlFilePath = string.Format(@"{0}\IntegrationTests\XmlResources\ValueType.xml", TestContext.CurrentContext.TestDirectory);

		[SetUp]
		protected override void SetUp()
		{
			base.SetUp();
		}

		[TearDown]
		public override void TearDown()
		{
			base.TearDown();
		}

		#endregion
		
		[Test]
		public void Xml_icerigindeki_type_tipindeki_member_sayisi_kadar_md_dosyasi_export_edilir()
		{
			//act
			var actual = testing.Execute("value-type", xmlFilePath, targetFolderPath);

			//assert
			Assert.AreEqual(0,actual);
			Assert.AreEqual(4,new DirectoryInfo(targetFolderPath).GetFiles().Length);
		}

		[Test]
		public void Export_edilen_dosyalarin_basliklari_dosya_adlaridir()
		{
			//act
			var actual = testing.Execute("value-type", xmlFilePath, targetFolderPath);

			//assert
			Assert.AreEqual(0,actual);

			var files = new DirectoryInfo(targetFolderPath).GetFiles();

			Assert.IsTrue(fileSystem.Read(files[0].FullName).StartsWith(string.Format("# {0}", files[0].Name.Split('.')[0])));
			Assert.IsTrue(fileSystem.Read(files[1].FullName).StartsWith(string.Format("# {0}", files[1].Name.Split('.')[0])));
			
		}

	}
}
