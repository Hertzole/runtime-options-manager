#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine;
using UnityEngine.Localization;

namespace Hertzole.OptionsManager
{
	public partial class BaseSetting
	{
		[SerializeField]
		private LocalizedString displayNameLocalized = default;
		
#if HERTZ_SETTINGS_LOCALIZATION
		public LocalizedString DisplayNameLocalized { get { return displayNameLocalized; } set { displayNameLocalized = value; } }
#endif
	}
}
#endif