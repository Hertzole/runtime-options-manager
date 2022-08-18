using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Assertions;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Hertzole.OptionsManager.Tests
{
	public class LocalizationTest : BaseTest
	{
		private List<Locale> locales;
		protected LocalizationSettings TargetLocalizationSettings { get; private set; }

		protected IReadOnlyList<Locale> Locales { get { return locales; } }

		protected override IEnumerator OnSetUpRoutine()
		{
			AsyncOperationHandle<IResourceLocator> addressablesOperation = UnityEngine.AddressableAssets.Addressables.InitializeAsync();
			while (!addressablesOperation.IsDone)
			{
				yield return null;
			}
			
			TargetLocalizationSettings = CreateLocalizationSettings();

			LocalesProvider localesProvider = CreateLocalesProvider();

			AsyncOperationHandle localesOperation = localesProvider.PreloadOperation;
			while (!localesOperation.IsDone)
			{
				yield return null;
			}
			
			List<Locale> newLocales = CreteLocales();

			for (int i = 0; i < newLocales.Count; i++)
			{
				localesProvider.AddLocale(newLocales[i]);
			}

			locales = localesProvider.Locales;

			TargetLocalizationSettings.SetAvailableLocales(localesProvider);
			TargetLocalizationSettings.GetStartupLocaleSelectors().Clear();
			TargetLocalizationSettings.GetStartupLocaleSelectors().Add(new SpecificLocaleSelector()
			{
				LocaleId = locales[0].Identifier
			});

			AsyncOperationHandle<LocalizationSettings> initializeOperation = TargetLocalizationSettings.GetInitializationOperation();

			while (!initializeOperation.IsDone)
			{
				yield return null;
			}
	
			Assert.AreEqual(1, TargetLocalizationSettings.GetStartupLocaleSelectors().Count);
		}

		protected override void OnTearDown()
		{
			Object.Destroy(TargetLocalizationSettings);
			locales.Clear();
		}

		protected override T AddSetting<T>(bool hasValueLimits = false)
		{
			T setting = base.AddSetting<T>(hasValueLimits);
			if (setting is LanguageSetting languageSetting)
			{
				languageSetting.TargetLocalizationSettings = TargetLocalizationSettings;
			}

			return setting;
		}

		protected LocalizationSettings CreateLocalizationSettings()
		{
			return ScriptableObject.CreateInstance<LocalizationSettings>();
		}

		protected LocalesProvider CreateLocalesProvider()
		{
			return new LocalesProvider();
		}

		protected List<Locale> CreteLocales()
		{
			return new List<Locale>()
			{
				Locale.CreateLocale("en-US"),
				Locale.CreateLocale("sv-SE"),
				Locale.CreateLocale("fr-FR"),
				Locale.CreateLocale("es-ES"),
				Locale.CreateLocale("de-DE")
			};
		}
	}
}