using System;
using System.Collections.Generic;
using UnityEngine;
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Hertzole.Settings
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Settings Object", menuName = "Hertzole/Settings/Settings Objects", order = -10000)]
#endif
	public class SettingsObject : ScriptableObject
	{
		[SerializeField]
		private SettingsCategory[] categories = default;

		private readonly Dictionary<string, Setting> cachedSettings = new Dictionary<string, Setting>();

		public SettingsCategory[] Categories { get { return categories; } set { categories = value; } }

		public bool TryGetSetting(string identifier, out Setting setting)
		{
			if (cachedSettings.TryGetValue(identifier, out setting))
			{
				return true;
			}

			for (int i = 0; i < categories.Length; i++)
			{
				for (int j = 0; j < categories[i].Settings.Length; j++)
				{
					if (categories[i].Settings[j].Identifier == identifier)
					{
						cachedSettings.Add(categories[i].Settings[j].Identifier, categories[i].Settings[j]);
						setting = categories[i].Settings[j];
						return true;
					}
				}
			}

			return false;
		}

		public Setting FindSetting(string identifier)
		{
			if (TryGetSetting(identifier, out Setting setting))
			{
				return setting;
			}

			Debug.LogError($"There's no setting with the identifier {identifier}.");
			return null;
		}
	}

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
		private Setting[] settings = default;

		public string DisplayName { get { return displayName; } set { displayName = value; } }
#if HERTZ_SETTINGS_LOCALIZATION
		public LocalizedString DisplayNameLocalized { get { return displayNameLocalized; } set { displayNameLocalized = value; } }
#endif
		public Setting[] Settings { get { return settings; } set { settings = value; } }
	}
}