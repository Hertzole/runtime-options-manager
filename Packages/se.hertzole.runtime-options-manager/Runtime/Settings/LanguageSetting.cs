#if HERTZ_SETTINGS_LOCALIZATION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Language Setting", menuName = "Hertzole/Settings/Language Setting")]
#endif
	public class LanguageSetting : Setting<Locale>, IDropdownValues
	{
		public enum DisplayType
		{
			CultureInfoName = 0,
			CultureInfoDisplayName = 1,
			CultureInfoNativeName = 2,
			CultureInfoEnglishName = 3,
			CustomName = 4
		}
		
		[SerializeField] 
		private DisplayType nameDisplayType = DisplayType.CultureInfoNativeName;
		[SerializeField] 
		private List<SerializableKeyValuePair<Locale, string>> customNames = new List<SerializableKeyValuePair<Locale, string>>();
		
		private (string, Sprite)[] cachedDropdownValues;

		private LocalizationSettings localizationSettings;

		public DisplayType NameDisplayType { get { return nameDisplayType; } set { nameDisplayType = value; } }

		public LocalizationSettings TargetLocalizationSettings
		{
			get
			{
				if (localizationSettings == null)
				{
					localizationSettings = LocalizationSettings.Instance;
				}

				return localizationSettings;
			}
			set { localizationSettings = value; }
		}

		public void AddCustomName(Locale locale, string localeName)
		{
			for (int i = 0; i < customNames.Count; i++)
			{
				if (customNames[i].Key == locale)
				{
					Debug.LogError($"Locale {locale} has already been added as a custom name.");
					return;
				}
			}

			customNames.Add(new SerializableKeyValuePair<Locale, string>(locale, localeName));
		}

		public bool RemoveCustomName(Locale locale)
		{
			for (int i = 0; i < customNames.Count; i++)
			{
				if (customNames[i].Key == locale)
				{
					customNames.RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		public string GetCustomName(Locale locale)
		{
			for (int i = 0; i < customNames.Count; i++)
			{
				if (customNames[i].Key == locale)
				{
					return customNames[i].Value;
				}
			}

			Debug.LogError($"Could not find a custom name for locale {locale}.");
			return locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.NativeName : locale.ToString();
		}

		public override object GetSerializeValue()
		{
			if (Value == null)
			{
				return DefaultValue.Identifier.Code;
			}
			
			Locale currentLocale = Value;
			return currentLocale.Identifier.Code;
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			DontInvokeSettingChanged = true;
			
			if (newValue is string stringValue)
			{
				SettingsManager.StartCoroutine(SetSerializedValueRoutine(stringValue));
			}
			else if (newValue is null)
			{
				SettingsManager.StartCoroutine(SetSerializedValueRoutine(string.Empty));
			}
			else
			{
				DontInvokeSettingChanged = false;
				base.SetSerializedValue(newValue, serializer);
			}
		}

		/// <summary>
		///     Set the value asynchronously due to needing to wait for the localization settings to be loaded.
		/// </summary>
		/// <param name="localeCode"></param>
		private IEnumerator SetSerializedValueRoutine(string localeCode)
		{
			if (TargetLocalizationSettings.GetAvailableLocales() is LocalesProvider localesProvider)
			{
				AsyncOperationHandle loadOperation = localesProvider.PreloadOperation;
				while (!loadOperation.IsDone)
				{
					yield return null;
				}
			}
			
			if (string.IsNullOrWhiteSpace(localeCode))
			{
				localeCode = TargetLocalizationSettings.GetSelectedLocale().Identifier.Code;
			}

			value = TryConvertValue(localeCode);
			DontInvokeSettingChanged = false;
		}

		protected override Locale TryConvertValue(object newValue)
		{
			if (newValue is string localeCode)
			{
				return TargetLocalizationSettings.GetAvailableLocales().GetLocale(new LocaleIdentifier(localeCode));
			}

			return TargetLocalizationSettings.GetSelectedLocale();
		}

		public void SetDropdownValue(int index)
		{
			Locale newLocale = TargetLocalizationSettings.GetAvailableLocales().Locales[index];
			Value = newLocale;
			TargetLocalizationSettings.SetSelectedLocale(newLocale);
		}

		public int GetDropdownValue()
		{
			return TargetLocalizationSettings.GetAvailableLocales().Locales.IndexOf(Value);
		}

		public IReadOnlyList<(string text, Sprite icon)> GetDropdownValues()
		{
			List<Locale> allLocales = TargetLocalizationSettings.GetAvailableLocales().Locales;
			if (cachedDropdownValues == null || cachedDropdownValues.Length != allLocales.Count)
			{
				cachedDropdownValues = new (string, Sprite)[allLocales.Count];
			}

			for (int i = 0; i < allLocales.Count; i++)
			{
				Locale locale = allLocales[i];
				string displayName = GetLocaleName(locale, nameDisplayType);
				cachedDropdownValues[i] = (displayName, null);
			}

			return cachedDropdownValues;
		}

		public string GetLocaleName(Locale locale, DisplayType displayType)
		{
			switch (displayType)
			{
				case DisplayType.CultureInfoName:
					return locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.Name : locale.ToString();
				case DisplayType.CultureInfoDisplayName:
					return locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.DisplayName : locale.ToString();
				case DisplayType.CultureInfoNativeName:
					return locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.NativeName : locale.ToString();
				case DisplayType.CultureInfoEnglishName:
					return locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.EnglishName : locale.ToString();
				case DisplayType.CustomName:
					for (int i = 0; i < customNames.Count; i++)
					{
						if (customNames[i].Key.Identifier == locale.Identifier)
						{
							return customNames[i].Value;
						}
					}
					
					return locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.NativeName : locale.ToString();
				default:
					return locale.ToString();
			}
		}
	}
}
#endif