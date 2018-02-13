using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Inventiv.Tools.XmlToMdConverter.Test.UnitTests
{
	[TestFixture]
	public class FileSystemTest
	{
		#region SetUp & Helpers

		private Tools.XmlToMdConverter.FileSystem.FileSystem testing;

		[SetUp]
		public void SetUp()
		{
			new DirectoryInfo(RootPath).Create();
			testing = new Tools.XmlToMdConverter.FileSystem.FileSystem();
		}

		private string RootPath
		{
			get
			{
				return Path.Combine(Path.GetTempPath(), "FileSystemTest");
			}
		}

		private string GeneratePath(string fileName = "")
		{
			fileName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid().ToString() : fileName;

			return string.Format(@"{0}\{1}", RootPath, fileName);
		}

		[TearDown]
		public void TearDown()
		{
			try
			{
				if (testing == null) { return; }

				new DirectoryInfo(RootPath).Delete(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Could not delete {0} on teardown: ", RootPath);
				Console.WriteLine(ex);
			}
		}

		#endregion

		[Test]
		public void Xml_dosyasinin_icerigi_okunabilir()
		{
			const string expected = "test string";
			var path = GeneratePath("Testing.xml");
			File.WriteAllText(path, expected);

			var actual = testing.Read(path);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Dosya_sisteminde_yeni_bir_dosya_olusturulabilir()
		{
			//arrange
			var path = GeneratePath();

			//act
			testing.Write(path, string.Empty);

			//assert
			Assert.IsTrue(testing.Exists(path));
		}

		[Test]
		public void Dosya_sistemindeki_md_dosyasinin_icerigi_silinip_yeni_icerik_yazilir()
		{
			//arrange
			const string expected = "test string 2";
			var path = GeneratePath("Testing.md");

			//act
			testing.Write(path, "some other text");
			testing.Write(path, expected);

			var actual = File.ReadAllText(path);

			//assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Dosya_sistemindeki_bir_Folderin_altindaki_tum_dosyalar_silinebilir()
		{
			//arrange
			var pathList = new List<string>()
			{
				GeneratePath(),
				GeneratePath(),
				GeneratePath()
			};

			pathList.ForEach(pl =>
			{
				File.Create(pl).Close();
			});

			//act
			testing.RemoveAll(RootPath);

			//assert
			var actual = Directory.GetFiles(RootPath);
			Assert.IsEmpty(actual);
		}
		
		[Test]
		public void Dosya_sistemindeki_md_dosyanina_icerik_eklenebilir()
		{
			//arrange
			var fileContent = "test1";
			var additionalContent = "test2";
			var path = GeneratePath();

			testing.Write(path, fileContent);

			//act
			testing.Append(path, additionalContent);

			//assert
			var readContent = testing.Read(path);
			Assert.AreEqual(string.Format("{0}{1}", fileContent, additionalContent), readContent);
		}

		[Test]
		public void Dosya_sistemindeki_md_dosyasinin_varligi_kontrol_edilebilir()
		{
			//arrange
			var path = GeneratePath();
			testing.Write(path, string.Empty);

			//act
			var actual = testing.Exists(path);

			//assert
			Assert.IsTrue(actual);

			actual = testing.Exists(string.Format("{0}test", path));
			Assert.IsFalse(actual);
		}

	}
}
