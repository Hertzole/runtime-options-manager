#if HERTZ_SETTINGS_LOCALIZATION
using System.Collections;
using System.IO;
using Hertzole.RuntimeOptionsManager.Localization;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.RuntimeOptionsManager.Tests
{
	public class LocalizationTests : BaseTest
	{
		private SettingsManager originalSettingsManager;
		private LocalizationSettings originalLocalizationSettings;

		protected override void OnPreSetup()
		{
			originalSettingsManager = SettingsManager.Instance;
			originalLocalizationSettings = LocalizationSettings.Instance;
		}

		protected override void OnTearDown()
		{
			SettingsManager.Instance = originalSettingsManager;
			LocalizationSettings.Instance = originalLocalizationSettings;
		}

		[UnityTest]
		public IEnumerator StartupSelector()
		{
			LocalizationSettings localSettings = ScriptableObject.CreateInstance<LocalizationSettings>();
			localSettings.name = "[TEST ONLY] LocalizationSettings";
			LocalesProvider localesProvider = new LocalesProvider();
			localSettings.SetAvailableLocales(localesProvider);

			LocalizationSettings.Instance = localSettings;

			yield return null;

			Locale en = ScriptableObject.CreateInstance<Locale>();
			en.Identifier = new LocaleIdentifier("en");
			Locale sv = ScriptableObject.CreateInstance<Locale>();
			sv.Identifier = new LocaleIdentifier("sv");

			AsyncOperationHandle providerInitialize = localesProvider.PreloadOperation;
			while (!providerInitialize.IsDone)
			{
				yield return null;
			}

			localesProvider.AddLocale(en);
			localesProvider.AddLocale(sv);

			LanguageSetting languageSetting = AddSetting<LanguageSetting>();
			languageSetting.Identifier = "language";
			languageSetting.Value = localesProvider.GetLocale("en");

			SettingsManager.Instance = settings;

			AsyncOperationHandle<LocalizationSettings> settingsInitialize = localSettings.GetInitializationOperation();
			while (!settingsInitialize.IsDone)
			{
				yield return null;
			}

			localSettings.SetSelectedLocale(en);

			Assert.AreEqual(localSettings.GetSelectedLocale().Identifier, en.Identifier);

			languageSetting.Value = sv;

			// yield return null;

			Assert.AreEqual(settings, SettingsManager.Instance);

			Assert.AreEqual(sv, languageSetting.Value);
			settings.SaveSettings();

			Assert.IsTrue(File.Exists(settings.ComputedSavePath));
			Assert.AreEqual(localSettings.GetSelectedLocale().Identifier, en.Identifier);

			SettingsLocalizationSelector selector = new SettingsLocalizationSelector
			{
				SettingIdentifier = "language"
			};

			localSettings.SetSelectedLocale(selector.GetStartupLocale(localesProvider));

			Assert.AreEqual(localSettings.GetSelectedLocale().Identifier, sv.Identifier);
		}

		[Test]
		public void StartupSelectorWithoutSettingsFile()
		{
			SettingsLocalizationSelector selector = new SettingsLocalizationSelector
			{
				SettingIdentifier = "language"
			};

			Locale result = selector.GetStartupLocale(null);

			Assert.AreEqual(null, result);
		}

		[UnityTest]
		public IEnumerator StartupSelectorWithoutSettingsManagers()
		{
			LocalizationSettings localSettings = ScriptableObject.CreateInstance<LocalizationSettings>();
			localSettings.name = "[TEST ONLY] LocalizationSettings";
			LocalesProvider localesProvider = new LocalesProvider();
			localSettings.SetAvailableLocales(localesProvider);

			LocalizationSettings.Instance = localSettings;

			yield return null;

			Locale en = ScriptableObject.CreateInstance<Locale>();
			en.Identifier = new LocaleIdentifier("en");
			Locale sv = ScriptableObject.CreateInstance<Locale>();
			sv.Identifier = new LocaleIdentifier("sv");

			AsyncOperationHandle providerInitialize = localesProvider.PreloadOperation;
			while (!providerInitialize.IsDone)
			{
				yield return null;
			}

			localesProvider.AddLocale(en);
			localesProvider.AddLocale(sv);

			LanguageSetting languageSetting = AddSetting<LanguageSetting>();
			languageSetting.Identifier = "language";
			languageSetting.Value = sv;

			settings.SaveSettings();
			string path = settings.ComputedSavePath;

			SettingsManager.Instance = null;

			SettingsLocalizationSelector selector = new SettingsLocalizationSelector
			{
				SettingIdentifier = "language"
			};

			Locale result = selector.GetStartupLocale(null);

			Assert.IsTrue(File.Exists(path));
			Assert.AreEqual(null, result);
		}
	}
}
#endif