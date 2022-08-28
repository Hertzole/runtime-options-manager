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

		private SettingsManager settingsManager;

		public string DisplayName { get { return displayName; } set { displayName = value; } }
#if HERTZ_SETTINGS_LOCALIZATION
		public LocalizedString DisplayNameLocalized { get { return displayNameLocalized; } set { displayNameLocalized = value; } }
#endif
		public Sprite Icon { get { return icon; } set { icon = value; } }
		public IReadOnlyList<BaseSetting> Settings { get { return settings; } }

		/// <summary>
		///     Initializes the category and all it's settings.
		/// </summary>
		/// <param name="newManager">The parent settings manager.</param>
		internal void Initialize(SettingsManager newManager)
		{
			settingsManager = newManager;

			for (int i = 0; i < settings.Count; i++)
			{
				settings[i].Initialize(newManager);
			}
		}

		/// <summary>
		///     Adds a new setting to the category.
		/// </summary>
		/// <param name="setting">The setting to add.</param>
		/// <param name="initialize">If true, the setting will be initialized when added.</param>
		public void AddSetting(BaseSetting setting, bool initialize = true)
		{
			InsertSetting(settings.Count, setting, initialize);
		}
		
		public void InsertSetting(int index, BaseSetting setting, bool initialize = true)
		{
			settings.Insert(index, setting);
			if (initialize)
			{
				setting.Initialize(settingsManager);
			}
		}

		/// <summary>
		///     Removes a setting from the category at the given index.
		/// </summary>
		/// <param name="settingIndex">The index of the setting to remove.</param>
		/// <returns>True if the setting was removed, otherwise false.</returns>
		public bool RemoveSetting(int settingIndex)
		{
			return RemoveSetting(settings[settingIndex]);
		}

		/// <summary>
		///     Removes a setting from the category.
		/// </summary>
		/// <param name="setting">The setting to remove.</param>
		/// <returns>True if the setting was removed, otherwise false.</returns>
		public bool RemoveSetting(BaseSetting setting)
		{
			return settings.Remove(setting);
		}
	}
}