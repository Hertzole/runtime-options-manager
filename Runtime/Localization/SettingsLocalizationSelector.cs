#if HERTZ_SETTINGS_LOCALIZATION
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Hertzole.SettingsManager.Localization
{
	[Serializable]
	public class SettingsLocalizationSelector : IStartupLocaleSelector
	{
		[SerializeField]
		private string settingIdentifier = "language";

		public string SettingIdentifier { get { return settingIdentifier; } set { settingIdentifier = value; } }

		public Locale GetStartupLocale(ILocalesProvider availableLocales)
		{
			Locale locale = null;

			// Load the settings to make sure they are loaded.
			SettingsManager.Instance.LoadSettingsIfNeeded();

			if (SettingsManager.Instance.HasLoadedSetting(settingIdentifier) && SettingsManager.Instance.TryGetSetting(settingIdentifier, out Setting setting) && setting is LanguageSetting language)
			{
				locale = language.Value;
			}

			return locale;
		}
	}
}
#endif