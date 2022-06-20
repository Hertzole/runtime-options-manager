using System;
using System.Collections.Generic;
using UnityEngine;
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Hertzole.OptionsManager
{
	[Serializable]
	public class SettingsCategory
	{
		[SerializeField]
		private string displayName = "New Category";
#if HERTZ_SETTINGS_LOCALIZATION
		[SerializeField]
		private LocalizedString displayNameLocalized = default;
#endif
		[SerializeField]
		private Sprite icon = default;
		[SerializeField]
		private List<BaseSetting> settings = new List<BaseSetting>();

		public string DisplayName { get { return displayName; } set { displayName = value; } }
#if HERTZ_SETTINGS_LOCALIZATION
		public LocalizedString DisplayNameLocalized { get { return displayNameLocalized; } set { displayNameLocalized = value; } }
#endif
		public Sprite Icon { get { return icon; } set { icon = value; } }
		public List<BaseSetting> Settings { get { return settings; } set { settings = value; } }
	}
}