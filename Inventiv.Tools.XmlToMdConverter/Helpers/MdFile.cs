namespace Inventiv.Tools.XmlToMdConverter.Helpers
{
	public class MdFile
	{
		public MdFile(string fileName, string content, bool isProtectIntroduction = false)
		{
			FileName = fileName;
			Content = content;
			IsProtectIntroduction = isProtectIntroduction;
		}

		public string FileName { get; private set; }
		public string Content { get; private set; }
		public bool IsProtectIntroduction { get; private set; }
	}
}