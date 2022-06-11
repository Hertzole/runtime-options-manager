using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hertzole.SettingsManager.Tests
{
	public class SerializationTest : BaseTest
	{
		private const string SAVE_PATH = "testing/settings";
		private const string FILE_NAME = "test_settings.json";
		private const string OVERRIDE_SAVE_PATH = "testing/override_settings";
		private const string OVERRIDE_FILE_NAME = "test_settings_override.txt";

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

		private static void CleanUp()
		{
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
				Directory.Delete(testingPath);
			}
		}

		[Test]
		public void GetSerializeValue_IntSetting()
		{
			TestSerializeValue<IntSetting, int>(1);
		}

		[Test]
		public void GetSerializeValue_FloatSetting()
		{
			TestSerializeValue<FloatSetting, float>(1.1f);
		}

		[Test]
		public void GetSerializeValue_BoolSetting()
		{
			TestSerializeValue<ToggleSetting, bool>(true);
		}

		[Test]
		public void GetSerializeValue_AudioSetting()
		{
			TestSerializeValue<AudioSetting, int>(1);
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
		public void IsUpdated_SaveLocation()
		{
			settings.SaveLocation = SettingsManager.SaveLocations.DataPath;

			IsUpdatedTest(() => { settings.SaveLocation = SettingsManager.SaveLocations.Documents; });
		}

		[Test]
		public void IsUpdated_SavePath()
		{
			settings.SavePath = SAVE_PATH;

			IsUpdatedTest(() => { settings.SavePath = OVERRIDE_SAVE_PATH; });
		}

		[Test]
		public void IsUpdated_FileName()
		{
			settings.FileName = FILE_NAME;

			IsUpdatedTest(() => { settings.FileName = OVERRIDE_FILE_NAME; });
		}

		[Test]
		public void SaveLocation_PersistentDataPath()
		{
			UnityEngine.Assertions.Assert.AreEqual(Application.persistentDataPath, SettingsManager.GetSaveLocation(SettingsManager.SaveLocations.PersistentDataPath, null));
		}

		[Test]
		public void SaveLocation_DataPath()
		{
			UnityEngine.Assertions.Assert.AreEqual(Application.dataPath, SettingsManager.GetSaveLocation(SettingsManager.SaveLocations.DataPath, null));
		}

		[Test]
		public void SaveLocation_Documents()
		{
			UnityEngine.Assertions.Assert.AreEqual(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SettingsManager.GetSaveLocation(SettingsManager.SaveLocations.Documents, null));
		}

		[Test]
		public void SaveLocation_Invalid()
		{
			LogAssert.Expect(LogType.Error, $"{int.MaxValue} is an invalid save location. Returning persistent data path instead.");
			UnityEngine.Assertions.Assert.AreEqual(Application.persistentDataPath, SettingsManager.GetSaveLocation((SettingsManager.SaveLocations) int.MaxValue, null));
		}

		private void TestSerializeValue<TSetting, TValue>(TValue newValue) where TSetting : Setting<TValue>, new()
		{
			TSetting setting = AddSetting<TSetting>();
			setting.Value = newValue;
			setting.GetSerializeValue();

			Assert.AreEqual(newValue, setting.Value);
		}

		private void IsUpdatedTest(Action setValues)
		{
			AddSetting<IntSetting>();
			string originalSavePath = settings.ComputedSavePath;

			settings.SaveSettings();

			Assert.IsTrue(File.Exists(originalSavePath));

			File.Delete(originalSavePath);

			setValues.Invoke();

			settings.SaveSettings();

			string newPath = SettingsManager.GetSaveLocation(settings.SaveLocation, settings.CustomPathProvider) + "/" + settings.SavePath + "/" + settings.FileName;

			Assert.IsTrue(File.Exists(newPath));

			File.Delete(newPath);
		}
	}
}