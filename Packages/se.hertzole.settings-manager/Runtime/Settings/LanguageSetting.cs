#if HERTZ_SETTINGS_LOCALIZATION
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif

namespace Hertzole.SettingsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Language Setting", menuName = "Hertzole/Settings/Language Setting")]
#endif
	public class LanguageSetting : Setting<Locale>, IDropdownValues
	{
		private (string, Sprite)[] cachedDropdownValues;
		
		public override object GetSerializeValue()
		{
			Locale currentLocale = Value;
			return currentLocale.Identifier.Code;
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			if (newValue is string stringValue)
			{
				value = TryConvertValue(stringValue);
			}
			else
			{
				base.SetSerializedValue(newValue, serializer);
			}
		}

		protected override Locale TryConvertValue(object newValue)
		{
			if (newValue is string localeCode)
			{
				Locale selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(localeCode));
				return selectedLocale;
			}

			return LocalizationSettings.SelectedLocale;
		}

		public void SetDropdownValue(int index)
		{
			LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.Instance.GetAvailableLocales().Locales[index]);
		}

		public int GetDropdownValue()
		{
			return LocalizationSettings.Instance.GetAvailableLocales().Locales.IndexOf(LocalizationSettings.Instance.GetSelectedLocale());
		}

		public IReadOnlyList<(string text, Sprite icon)> GetDropdownValues()
		{
			List<Locale> allLocales = LocalizationSettings.Instance.GetAvailableLocales().Locales;
			if (cachedDropdownValues == null || cachedDropdownValues.Length != allLocales.Count)
			{
				cachedDropdownValues = new (string, Sprite)[allLocales.Count];
			}
			
			for (int i = 0; i < allLocales.Count; i++)
			{
				Locale locale = allLocales[i];
				string displayName = locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.NativeName : locale.ToString();
				cachedDropdownValues[i] = (displayName, null);
			}

			return cachedDropdownValues;
		}

#if HERTZ_SETTINGS_UIELEMENTS && UNITY_2021_2_OR_NEWER
		public override VisualElement CreateUIElement()
		{
			if (uiElement == null)
			{
				return null;
			}

			TemplateContainer ui = uiElement.CloneTree();

			DropdownField dropdown = ui.Q<DropdownField>();
			dropdown.choices.Clear();
			dropdown.label = DisplayName;

			AsyncOperationHandle<Locale> localesOperation = LocalizationSettings.SelectedLocaleAsync;
			if (localesOperation.IsDone)
			{
				OnLanguageFinishedUIElements(localesOperation.Result, dropdown);
			}
			else
			{
				localesOperation.Completed += locale => OnLanguageFinishedUIElements(locale.Result, dropdown);
			}

			return ui;
		}

		private static void OnLanguageFinishedUIElements(Locale selectedLocale, DropdownField dropdown)
		{
			List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
			List<string> choices = new List<string>(locales.Count);
			int selectedOption = 0;

			for (int i = 0; i < locales.Count; i++)
			{
				if (selectedLocale.Equals(locales[i]))
				{
					selectedOption = i;
				}

				string displayName = locales[i].Identifier.CultureInfo != null ? locales[i].Identifier.CultureInfo.NativeName : locales[i].ToString();
				choices.Add(displayName);
			}

			dropdown.choices.Clear();
			dropdown.choices.AddRange(choices);
			dropdown.SetValueWithoutNotify(choices[selectedOption]);
			dropdown.RegisterValueChangedCallback(evt => { LocalizationSettings.SelectedLocale = locales[dropdown.index]; });
		}
#endif
	}
}
#endif