using System;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class SavingTests : BaseTest
	{
		private static readonly Type[] settingsTypes =
		{
			typeof(AudioSetting),
			typeof(FloatSetting),
			typeof(FullscreenModeSetting),
			typeof(InputSetting),
			typeof(IntSetting),
			typeof(LanguageSetting),
			typeof(RefreshRateSetting),
			typeof(ResolutionSetting),
			typeof(ToggleSetting),
			typeof(VSyncSetting)
		};

		[Test]
		public void RespectsCanSave([ValueSource(nameof(settingsTypes))] Type value)
		{
			// Add a dummy setting to make sure the file is saved.
			IntSetting dummySetting = AddSetting<IntSetting>();
			dummySetting.Identifier = "dummy";

			Setting setting = ScriptableObject.CreateInstance(value) as Setting;
			Assert.IsNotNull(setting);
			setting.Identifier = "test_setting";

			bool canSave = setting.CanSave;

			settings.Categories[0].AddSetting(setting);

			settings.SaveSettings();

			AssertHasSavedSettings();

			settings.LoadSettings();

			if (canSave)
			{
				Assert.IsTrue(settings.HasLoadedSetting(setting.Identifier));
			}
			else
			{
				Assert.IsFalse(settings.HasLoadedSetting(setting.Identifier));
			}
		}
	}
}