using System;
using System.IO;

namespace Inventiv.Tools.XmlToMdConverter.FileSystem
{
	public class FileSystem : IFileSystem
	{
		public string Read(string path)
		{
			if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(); }

			if (!File.Exists(path)) { throw new FileNotFoundException(path); }

			return File.ReadAllText(path);
		}

		public void Write(string path, string contents)
		{
			if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(); }

			File.WriteAllText(path, contents);
		}

		public void RemoveAll(string folderPath)
		{
			if (string.IsNullOrWhiteSpace(folderPath)) { throw new ArgumentNullException(); }

			var directoryInfo = new DirectoryInfo(folderPath);

			foreach (var file in directoryInfo.GetFiles())
			{
				file.Delete();
			}
		}
		
		public void Append(string path, string additionalContent)
		{
			File.AppendAllText(path, additionalContent);
		}

		public bool Exists(string path)
		{
			return File.Exists(path);
		}
	}
}
