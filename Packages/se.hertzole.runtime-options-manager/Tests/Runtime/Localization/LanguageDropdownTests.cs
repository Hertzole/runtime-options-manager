#if HERTZ_SETTINGS_LOCALIZATION
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class LanguageDropdownTests : LocalizationTest
	{
		[Test]
		public void DropdownValuesMatch()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();
			setting.NameDisplayType = LanguageSetting.DisplayType.CultureInfoName;
			IReadOnlyList<(string text, Sprite icon)> dropdownValues = setting.GetDropdownValues();
			
			Assert.AreEqual(Locales.Count, dropdownValues.Count);

			for (int i = 0; i < Locales.Count; i++)
			{
				Assert.AreEqual(Locales[i].Identifier.CultureInfo.Name, dropdownValues[i].text);
			}
		}

		[Test]
		public void GetDropdownValueMatch()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				setting.Value = Locales[i];
				
				Assert.AreEqual(i, setting.GetDropdownValue());
			}
		}

		[Test]
		public void SetDropdownValueMatch()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				setting.SetDropdownValue(i);

				Assert.AreEqual(Locales[i].Identifier, setting.Value.Identifier);
				Assert.AreEqual(TargetLocalizationSettings.GetSelectedLocale().Identifier, setting.Value.Identifier);
			}
		}
	}
}
#endif