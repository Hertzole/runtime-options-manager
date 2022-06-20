using System;
using System.IO;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class SaveAndLoadTests : BaseTest
	{
		[Test]
		public void SaveWithBaseSetting()
		{
			AddSetting<ButtonSetting>();
			SerializeTest<IntSetting, int>(69);
		}

		[Test]
		public void SaveWithOnlyBaseSetting()
		{
			AddSetting<ButtonSetting>();

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			
			settings.SaveSettings();

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
		}

		[Test]
		public void SetDefaultValueOnNewSetting()
		{
			IntSetting oldSetting = AddSetting<IntSetting>();
			oldSetting.DefaultValue = 42;
			oldSetting.Value = 10;
			oldSetting.Identifier = "old_setting";
		
			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
		
			settings.SaveSettings();
		
			Assert.IsTrue(File.Exists(settings.ComputedSavePath));

			IntSetting newSetting = AddSetting<IntSetting>();
			newSetting.DefaultValue = 69;
			newSetting.Value = 20;
			newSetting.Identifier = "new_setting";
			
			settings.LoadSettings();
			
			Assert.AreEqual(69, newSetting.Value);
		}
		
		private void SerializeTest<TSetting, TValue>(TValue newValue) where TSetting : Setting<TValue> where TValue : IEquatable<TValue>
		{
			TSetting setting = AddSetting<TSetting>();

			setting.Identifier = "setting";
			setting.Value = newValue;

			Assert.AreNotEqual(default, newValue, "You should not provide the default value as it needs something else to check against!");
			Assert.AreEqual(newValue, setting.Value);
			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			
			settings.SaveSettings();

			Assert.IsTrue(File.Exists(settings.ComputedSavePath));
			
			setting.Value = default;
			Assert.AreNotEqual(newValue, setting.Value);
			
			settings.LoadSettings();
			
			Assert.AreEqual(newValue, setting.Value);
		}
	}
}