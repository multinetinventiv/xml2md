namespace Inventiv.Tools.XmlToMdConverter.FileSystem
{
	public interface IFileSystem
	{
		string Read(string path);
		void Write(string path, string contents);
		void RemoveAll(string folderPath);
		void Append(string path, string additionalContent);
		bool Exists(string path);
	}
}