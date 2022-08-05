using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class LanguageValues : LocalizationTest
	{
		[Test]
		public void GetSerializableValue_NullValue()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();
			setting.DefaultValue = Locales[0];
			setting.Value = null;

			Assert.AreEqual(Locales[0].Identifier.Code, setting.GetSerializeValue());
		}

		[Test]
		public void GetSerializableValue_NotNullValue()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();
			setting.DefaultValue = Locales[0];
			setting.Value = Locales[1];

			Assert.AreEqual(Locales[1].Identifier.Code, setting.GetSerializeValue());
		}

		[Test]
		public void SetSerializableValue_Code()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			setting.SetSerializedValue(Locales[0].Identifier.Code, new JsonSettingSerializer());
			
			Assert.AreEqual(Locales[0].Identifier, setting.Value.Identifier);
			Assert.AreEqual(TargetLocalizationSettings.GetSelectedLocale().Identifier, setting.Value.Identifier);
		}
		
		[Test]
		public void SetSerializableValue_Locale()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			setting.SetSerializedValue(Locales[0], new JsonSettingSerializer());
			
			Assert.AreEqual(Locales[0].Identifier, setting.Value.Identifier);
			Assert.AreEqual(TargetLocalizationSettings.GetSelectedLocale().Identifier, setting.Value.Identifier);
		}
		
		[Test]
		public void SetSerializableValue_Null()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			setting.SetSerializedValue(null, new JsonSettingSerializer());
			
			Assert.AreEqual(TargetLocalizationSettings.GetSelectedLocale().Identifier, setting.Value.Identifier);
		}
		
		[Test]
		public void SetSerializableValue_Invalid()
		{
			LanguageSetting setting = AddSetting<LanguageSetting>();

			setting.SetSerializedValue(42, new JsonSettingSerializer());
			
			Assert.AreEqual(TargetLocalizationSettings.GetSelectedLocale().Identifier, setting.Value.Identifier);
		}
	}
}