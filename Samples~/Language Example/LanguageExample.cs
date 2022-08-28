using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Hertzole.OptionsManager.Samples.Language
{
	public class LanguageExample : MonoBehaviour
	{
		[SerializeField]
		private SettingsManager settingsManager = default;
		[SerializeField]
		private LanguageSetting languageSetting = default;
		[SerializeField]
		private Dropdown dropdown = default;
		[SerializeField]
		private LocalizationSettings localizationSettings = default;

		private LocalizationSettings previousLocalizationSettings;

		private void Awake()
		{
			// If no settings are assigned, use the global one.
			if (settingsManager == null)
			{
				settingsManager = SettingsManager.Instance;
			}
			else
			{
				// When you have custom settings, it's important to initialize them.
				settingsManager.Initialize();
			}

			previousLocalizationSettings = LocalizationSettings.Instance;
			LocalizationSettings.Instance = localizationSettings;
			languageSetting.TargetLocalizationSettings = localizationSettings;

			dropdown.onValueChanged.AddListener(OnDropdownChanged);
		}

		private IEnumerator Start()
		{
			if (!(localizationSettings.GetAvailableLocales() is LocalesProvider locales))
			{
				yield break;
			}

			AsyncOperationHandle preloadOperation = locales.PreloadOperation;
			while (!preloadOperation.IsDone)
			{
				yield return null;
			}

			// This is a very important delay!
			// There can be a slight delay before the serialized value of language settings get set, so we must wait
			// a frame before we can get the current language value.
			yield return null;

			dropdown.options.Clear();

			IReadOnlyList<(string text, Sprite icon)> dropdownValues = languageSetting.GetDropdownValues();
			foreach ((string text, Sprite icon) dropdownValue in dropdownValues)
			{
				dropdown.options.Add(new Dropdown.OptionData(dropdownValue.text, dropdownValue.icon));
			}

			dropdown.RefreshShownValue();
			dropdown.SetValueWithoutNotify(languageSetting.GetDropdownValue());
		}

		private void OnDisable()
		{
			LocalizationSettings.Instance = previousLocalizationSettings;
		}

		private void OnDropdownChanged(int index)
		{
			languageSetting.SetDropdownValue(index);
		}
	}
}