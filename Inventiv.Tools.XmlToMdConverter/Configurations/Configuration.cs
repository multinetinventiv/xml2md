namespace Inventiv.Tools.XmlToMdConverter.Configurations
{
	public class Configuration : IConfiguration
	{
		public Configuration()
		{
			this.IsIncludedCaption = true;
		}

		public bool IsIncludedCaption { get; private set; }

		public void SetConfiguration(bool isIncludedCaption)
		{
			this.IsIncludedCaption = isIncludedCaption;
		}
	}
}
