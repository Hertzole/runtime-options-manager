using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Hertzole.SettingsManager
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
		private List<Setting> settings = new List<Setting>();

		public string DisplayName { get { return displayName; } set { displayName = value; } }
#if HERTZ_SETTINGS_LOCALIZATION
		public LocalizedString DisplayNameLocalized { get { return displayNameLocalized; } set { displayNameLocalized = value; } }
#endif
		public Sprite Icon { get { return icon; } set { icon = value; } }
		public List<Setting> Settings { get { return settings; } set { settings = value; } }
	}
}