#if HERTZ_SETTINGS_LOCALIZATION
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class LanguageNames : LocalizationTest
	{
		[Test]
		public void CultureInfoName()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				string name = setting.GetLocaleName(Locales[i], LanguageSetting.DisplayType.CultureInfoName);
				Assert.AreEqual(Locales[i].Identifier.CultureInfo.Name, name);
			}
		}
		
		[Test]
		public void CultureInfoDisplayName()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				string name = setting.GetLocaleName(Locales[i], LanguageSetting.DisplayType.CultureInfoDisplayName);
				Assert.AreEqual(Locales[i].Identifier.CultureInfo.DisplayName, name);
			}
		}
		
		[Test]
		public void CultureInfoNativeName()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				string name = setting.GetLocaleName(Locales[i], LanguageSetting.DisplayType.CultureInfoNativeName);
				Assert.AreEqual(Locales[i].Identifier.CultureInfo.NativeName, name);
			}
		}
		
		[Test]
		public void CultureInfoEnglishName()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				string name = setting.GetLocaleName(Locales[i], LanguageSetting.DisplayType.CultureInfoEnglishName);
				Assert.AreEqual(Locales[i].Identifier.CultureInfo.EnglishName, name);
			}
		}
		
		[Test]
		public void CultureInfoCustomNames()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				setting.AddCustomName(Locales[i], $"{Locales[i].Identifier.CultureInfo.EnglishName} (Custom {i})");
			}

			for (int i = 0; i < Locales.Count; i++)
			{
				string name = setting.GetLocaleName(Locales[i], LanguageSetting.DisplayType.CustomName);
				Assert.AreEqual($"{Locales[i].Identifier.CultureInfo.EnglishName} (Custom {i})", name);
			}
		}
		
		[Test]
		public void CultureInfoCustomNamesNoLocales()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				string name = setting.GetLocaleName(Locales[i], LanguageSetting.DisplayType.CustomName);
				Assert.AreEqual(Locales[i].Identifier.CultureInfo.NativeName, name);
			}
		}
		
		[Test]
		public void UnsupportedDisplayName()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				string name = setting.GetLocaleName(Locales[i], (LanguageSetting.DisplayType) int.MaxValue);
				Assert.AreEqual(Locales[i].ToString(), name);
			}
		}

		[Test]
		public void AddCustomNameAlreadyExists()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				setting.AddCustomName(Locales[i], $"{Locales[i].Identifier.CultureInfo.EnglishName} (Custom {i})");
			}
			
			for (int i = 0; i < Locales.Count; i++)
			{
				LogAssert.Expect(LogType.Error, $"Locale {Locales[i]} has already been added as a custom name.");
				setting.AddCustomName(Locales[i], $"{Locales[i].Identifier.CultureInfo.EnglishName} (Custom {i})");
			}
		}
		
		[Test]
		public void RemoveCustomNames()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				setting.AddCustomName(Locales[i], $"{Locales[i].Identifier.CultureInfo.EnglishName} (Custom {i})");
			}
			
			for (int i = 0; i < Locales.Count; i++)
			{
				Assert.IsTrue(setting.RemoveCustomName(Locales[i]));
			}
		}
		
		[Test]
		public void RemoveCustomNamesInvalid()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				Assert.IsFalse(setting.RemoveCustomName(Locales[i]));
			}
		}
		
		[Test]
		public void GetCustomName()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			for (int i = 0; i < Locales.Count; i++)
			{
				setting.AddCustomName(Locales[i], $"{Locales[i].Identifier.CultureInfo.EnglishName} (Custom {i})");
			}
			
			for (int i = 0; i < Locales.Count; i++)
			{
				Assert.AreEqual($"{Locales[i].Identifier.CultureInfo.EnglishName} (Custom {i})", setting.GetCustomName(Locales[i]));
			}
		}
		
		[Test]
		public void GetCustomNameInvalid()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();
			
			for (int i = 0; i < Locales.Count; i++)
			{
				LogAssert.Expect(LogType.Error, $"Could not find a custom name for locale {Locales[i]}.");
				Assert.AreEqual(Locales[i].Identifier.CultureInfo.NativeName, setting.GetCustomName(Locales[i]));
			}
		}
	}
}
#endif