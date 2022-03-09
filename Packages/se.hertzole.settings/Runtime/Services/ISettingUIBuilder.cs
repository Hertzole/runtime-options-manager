namespace Hertzole.Settings
{
	public interface ISettingUIBuilder
	{
		void CreateCategory(SettingsCategory category);

		void CreateElement(Setting setting);
	}
}