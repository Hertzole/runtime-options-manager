#if HERTZ_SETTINGS_INPUTSYSTEM // Only used when the new input system is installed.
using System.Collections.Generic;

namespace Hertzole.SettingsManager
{
	public struct InputActionData
	{
		public readonly string id;
		public readonly List<InputBindingData> bindings;

		public InputActionData(string id, IEnumerable<InputBindingData> bindings)
		{
			this.id = id;
			this.bindings = new List<InputBindingData>(bindings);
		}
	}
}
#endif