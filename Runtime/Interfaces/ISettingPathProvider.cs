namespace Hertzole.OptionsManager
{
	public interface ISettingPathProvider
	{
		string GetFullSettingsPath(string path, string fileName);
	}
}