using System.IO;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class OverwritePathTests : BaseTest
	{
		public const string SAVE_PATH = "testing/settings";
		public const string FILE_NAME = "test_settings.json";
		public const string OVERRIDE_SAVE_PATH = "testing/override_settings";
		public const string OVERRIDE_FILE_NAME = "test_settings_override.txt";

		protected override void OnSetup()
		{
			settings.SaveLocation = SettingsManager.SaveLocations.DataPath;
			settings.SavePath = SAVE_PATH;
			settings.FileName = FILE_NAME;

			CleanUp();
		}

		protected override void OnTearDown()
		{
			CleanUp();
		}

		protected override void CleanUp()
		{
			base.CleanUp();
			
			string savePath = Application.dataPath + "/" + SAVE_PATH;

			if (Directory.Exists(savePath))
			{
				Directory.Delete(savePath, true);
			}

			string overridenSavePath = Application.dataPath + "/" + OVERRIDE_SAVE_PATH;

			if (Directory.Exists(overridenSavePath))
			{
				Directory.Delete(overridenSavePath, true);
			}

			string testingPath = Application.dataPath + "/testing";

			if (Directory.Exists(testingPath))
			{
				Directory.Delete(testingPath, true);
			}
		}

		[Test]
		public void OverwriteSavePath()
		{
			string overridenSavePath = Application.dataPath + "/" + OVERRIDE_SAVE_PATH;

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsFalse(Directory.Exists(overridenSavePath));

			IntSetting setting = AddSetting<IntSetting>();
			setting.OverwriteSavePath = true;
			setting.OverriddenSavePath = OVERRIDE_SAVE_PATH;

			settings.SaveSettings();

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsTrue(File.Exists(overridenSavePath + "/" + FILE_NAME));
		}

		[Test]
		public void OverwriteFileName()
		{
			string savePath = Application.dataPath + "/" + SAVE_PATH;

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsFalse(Directory.Exists(savePath));

			IntSetting setting = AddSetting<IntSetting>();
			setting.OverwriteFileName = true;
			setting.OverriddenFileName = OVERRIDE_FILE_NAME;

			settings.SaveSettings();

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsTrue(File.Exists(savePath + "/" + OVERRIDE_FILE_NAME));
		}

		[Test]
		public void OverwriteSavePathAndFileName()
		{
			string path = Application.dataPath + "/" + OVERRIDE_SAVE_PATH + "/" + OVERRIDE_FILE_NAME;

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsFalse(File.Exists(path));

			IntSetting setting = AddSetting<IntSetting>();
			setting.OverwriteSavePath = true;
			setting.OverriddenSavePath = OVERRIDE_SAVE_PATH;
			setting.OverwriteFileName = true;
			setting.OverriddenFileName = OVERRIDE_FILE_NAME;

			settings.SaveSettings();

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsTrue(File.Exists(path));
		}

		[Test]
		public void OverwriteSavePathWithNonOverwrite()
		{
			string overridenSavePath = Application.dataPath + "/" + OVERRIDE_SAVE_PATH;

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsFalse(Directory.Exists(overridenSavePath));

			IntSetting overwriteSetting = AddSetting<IntSetting>();
			overwriteSetting.OverwriteSavePath = true;
			overwriteSetting.OverriddenSavePath = OVERRIDE_SAVE_PATH;
			overwriteSetting.Identifier = "overwrite";
			IntSetting nonOverwriteSetting = AddSetting<IntSetting>();
			nonOverwriteSetting.OverwriteSavePath = false;
			nonOverwriteSetting.Identifier = "standard";

			settings.SaveSettings();

			Assert.IsTrue(File.Exists(settings.ComputedSavePath));
			Assert.IsTrue(File.Exists(overridenSavePath + "/" + FILE_NAME));
		}

		[Test]
		public void OverwriteFileNameWithNonOverwrite()
		{
			string savePath = Application.dataPath + "/" + SAVE_PATH;

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsFalse(Directory.Exists(savePath));

			IntSetting overwriteSetting = AddSetting<IntSetting>();
			overwriteSetting.OverwriteFileName = true;
			overwriteSetting.OverriddenFileName = OVERRIDE_FILE_NAME;
			overwriteSetting.Identifier = "overwrite";
			IntSetting nonOverwriteSetting = AddSetting<IntSetting>();
			nonOverwriteSetting.OverwriteFileName = false;
			nonOverwriteSetting.Identifier = "standard";

			settings.SaveSettings();

			Assert.IsTrue(File.Exists(settings.ComputedSavePath));
			Assert.IsTrue(File.Exists(savePath + "/" + FILE_NAME));
		}

		[Test]
		public void OverwriteSavePathAndFileNameWithNonOverwrite()
		{
			string path = Application.dataPath + "/" + OVERRIDE_SAVE_PATH + "/" + OVERRIDE_FILE_NAME;

			Assert.IsFalse(File.Exists(settings.ComputedSavePath));
			Assert.IsFalse(File.Exists(path));

			IntSetting overwriteSetting = AddSetting<IntSetting>();
			overwriteSetting.OverwriteSavePath = true;
			overwriteSetting.OverriddenSavePath = OVERRIDE_SAVE_PATH;
			overwriteSetting.OverwriteFileName = true;
			overwriteSetting.OverriddenFileName = OVERRIDE_FILE_NAME;
			overwriteSetting.Identifier = "overwrite";
			IntSetting nonOverwriteSetting = AddSetting<IntSetting>();
			nonOverwriteSetting.OverwriteSavePath = false;
			nonOverwriteSetting.OverwriteFileName = false;
			nonOverwriteSetting.Identifier = "standard";

			settings.SaveSettings();

			Assert.IsTrue(File.Exists(settings.ComputedSavePath));
			Assert.IsTrue(File.Exists(path));
		}

		[Test]
		public void LoadMultipleSettingFiles()
		{
			string path = Application.dataPath + "/" + OVERRIDE_SAVE_PATH + "/" + OVERRIDE_FILE_NAME;

			Assert.IsFalse(File.Exists(settings.ComputedSavePath), "Settings already exists.");
			Assert.IsFalse(File.Exists(path), "Overwrite settings already exists.");
			
			IntSetting setting = AddSetting<IntSetting>();
			setting.OverwriteSavePath = true;
			setting.OverriddenSavePath = OVERRIDE_SAVE_PATH;
			setting.OverwriteFileName = true;
			setting.OverriddenFileName = OVERRIDE_FILE_NAME;
			setting.Identifier = "overwrite";
			IntSetting nonOverwriteSetting = AddSetting<IntSetting>();
			nonOverwriteSetting.OverwriteSavePath = false;
			nonOverwriteSetting.OverwriteFileName = false;
			nonOverwriteSetting.Identifier = "standard";

			setting.Value = 10;
			nonOverwriteSetting.Value = 20;
			
			settings.SaveSettings();

			Assert.IsTrue(File.Exists(settings.ComputedSavePath), "Settings file not created.");
			Assert.IsTrue(File.Exists(path), "Overwrite settings file not created.");
			
			setting.Value = 0;
			nonOverwriteSetting.Value = 0;
			
			settings.LoadSettings();
			
			Assert.AreEqual(10, setting.Value, "Overwrite settings not loaded and applied.");
			Assert.AreEqual(20, nonOverwriteSetting.Value, "Settings not loaded and applied.");
		}
	}
}