namespace Inventiv.Tools.XmlToMdConverter.Configurations
{
	public interface IConfiguration
	{
		bool IsIncludedCaption { get; }

		void SetConfiguration(bool isIncludedCaption);
	}
}
