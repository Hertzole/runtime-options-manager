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
			bool firstTimeSetup = modes.Count == 0;

			List<(LocalizedString, Sprite)> list = new List<(LocalizedString, Sprite)>(4);

			if (exclusiveFullscreen.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.ExclusiveFullScreen);
				}

				list.Add((exclusiveFullscreen.localizedName, null));
			}

			if (borderlessFullscreen.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.FullScreenWindow);
				}

				list.Add((borderlessFullscreen.localizedName, null));
			}

			if (maximizedWindow.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.MaximizedWindow);
				}

				list.Add((maximizedWindow.localizedName, null));
			}

			if (windowed.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.Windowed);
				}

				list.Add((windowed.localizedName, null));
			}

			return list;
		}
	}
}
#endif