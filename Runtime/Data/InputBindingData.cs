#if HERTZ_SETTINGS_INPUTSYSTEM // Only used when the new input system is installed.
namespace Hertzole.Settings
{
	public struct InputBindingData
	{
		public readonly string id;
		public readonly string path;
		public readonly bool isOverwritten;

		public InputBindingData(string id, string path, bool isOverwritten)
		{
			this.id = id;
			this.path = path;
			this.isOverwritten = isOverwritten;
		}
	}
}
#endif