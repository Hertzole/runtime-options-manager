#if HERTZ_SETTINGS_LOCALIZATION
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Hertzole.OptionsManager
{
	public interface IDropdownValuesLocalized : IDropdownValues
	{
		IReadOnlyList<(LocalizedString text, Sprite icon)> GetLocalizedDropdownValues();
	}
}
#endif