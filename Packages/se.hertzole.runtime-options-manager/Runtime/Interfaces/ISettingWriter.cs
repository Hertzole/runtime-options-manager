namespace Hertzole.OptionsManager
{
	public interface ISettingWriter
	{
		bool FileExists(string path);

		void WriteFile(string path, byte[] content);

		byte[] ReadFile(string path);
	}
}