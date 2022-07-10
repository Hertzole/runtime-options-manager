#if HERTZ_SETTINGS_LOCALIZATION
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Hertzole.OptionsManager
{
	public partial class FullscreenModeSetting : IDropdownValuesLocalized
	{
		public IReadOnlyList<(LocalizedString text, Sprite icon)> GetLocalizedDropdownValues()
		{
			GetModes();

			(LocalizedString, Sprite)[] result = new (LocalizedString, Sprite)[modes.Count];

			for (int i = 0; i < modes.Count; i++)
			{
				result[i] = (modes[i].localizedName, modes[i].icon);
			}

			return result;
		}
	}
}
#endif