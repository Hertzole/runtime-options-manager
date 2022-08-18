using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class SerializationTest : BaseTest
	{
		protected override void CleanUp()
		{
			base.CleanUp();

			string savePath = Application.dataPath + "/" + OverwritePathTests.SAVE_PATH;

			if (Directory.Exists(savePath))
			{
				Directory.Delete(savePath, true);
			}

			string overridenSavePath = Application.dataPath + "/" + OverwritePathTests.OVERRIDE_SAVE_PATH;

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
		public void GetSerializeValue_IntSetting([ValueSource(nameof(intValues))] int value)
		{
			TestSerializeValue<IntSetting, int>(value);
		}

		[Test]
		public void GetSerializeValue_FloatSetting([ValueSource(nameof(floatValues))] float value)
		{
			TestSerializeValue<FloatSetting, float>(value);
		}

		[Test]
		public void GetSerializeValue_BoolSetting([ValueSource(nameof(boolValues))] bool value)
		{
			TestSerializeValue<ToggleSetting, bool>(value);
		}

		[Test]
		public void GetSerializeValue_AudioSetting([ValueSource(nameof(intValues))] int value)
		{
			TestSerializeValue<AudioSetting, int>(value);
		}

		[Test]
		public void GetSerializeValue_VsyncSetting([ValueSource(nameof(boolValues))] bool value)
		{
			TestSerializeValue<VSyncSetting, bool>(value);
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
			settings.SavePath = OverwritePathTests.SAVE_PATH;

			IsUpdatedTest(() => { settings.SavePath = OverwritePathTests.OVERRIDE_SAVE_PATH; });
		}

		[Test]
		public void IsUpdated_FileName()
		{
			settings.FileName = OverwritePathTests.FILE_NAME;

			IsUpdatedTest(() => { settings.FileName = OverwritePathTests.OVERRIDE_FILE_NAME; });
		}

		[Test]
		public void SaveLocation_PersistentDataPath()
		{
			Assert.AreEqual(Application.persistentDataPath, SettingsManager.GetSaveLocation(SettingsManager.SaveLocations.PersistentDataPath, null, string.Empty, string.Empty));
		}

		[Test]
		public void SaveLocation_DataPath()
		{
			Assert.AreEqual(Application.dataPath, SettingsManager.GetSaveLocation(SettingsManager.SaveLocations.DataPath, null, string.Empty, string.Empty));
		}

		[Test]
		public void SaveLocation_Documents()
		{
			Assert.AreEqual(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SettingsManager.GetSaveLocation(SettingsManager.SaveLocations.Documents, null, string.Empty, string.Empty));
		}

		[Test]
		public void SaveLocation_Invalid()
		{
			LogAssert.Expect(LogType.Error, $"{int.MaxValue.ToString()} is an invalid save location. Returning persistent data path instead.");
			Assert.AreEqual(Application.persistentDataPath, SettingsManager.GetSaveLocation((SettingsManager.SaveLocations) int.MaxValue, null, string.Empty, string.Empty));
		}
		
		[Test]
		public void SetSerializerUpdates()
		{
			ISettingSerializer serializer = new JsonSettingSerializer();
			settings.Serializer = serializer;
			Assert.AreEqual(serializer, settings.Serializer);
			Assert.AreEqual(serializer, settings.behavior.Serializer);
			
			ISettingSerializer dummySerializer = new DummySerializer();
			settings.Serializer = dummySerializer;
			Assert.AreEqual(dummySerializer, settings.Serializer);
			Assert.AreEqual(dummySerializer, settings.behavior.Serializer);
		}

		private void TestSerializeValue<TSetting, TValue>(TValue newValue) where TSetting : Setting<TValue>, new()
		{
			TSetting setting = AddSetting<TSetting>(false);
			setting.Value = newValue;
			setting.GetSerializeValue();

			Assert.AreEqual(newValue, setting.Value);
		}

		private void IsUpdatedTest(Action setValues)
		{
			AddSetting<IntSetting>();
			string originalSavePath = settings.ComputedSavePath;

			settings.SaveSettings();

			NUnit.Framework.Assert.IsTrue(File.Exists(originalSavePath));

			File.Delete(originalSavePath);

			setValues.Invoke();

			settings.SaveSettings();

			string newPath = SettingsManager.GetSaveLocation(settings.SaveLocation, settings.CustomPathProvider, settings.SavePath, settings.FileName);

			NUnit.Framework.Assert.IsTrue(File.Exists(newPath));

			File.Delete(newPath);
		}
		
		private class DummySerializer : ISettingSerializer
		{
			public byte[] Serialize(Dictionary<string, object> data)
			{
				throw new NotImplementedException();
			}

			public void Deserialize(byte[] data, Dictionary<string, object> dataToFill)
			{
				throw new NotImplementedException();
			}

			public T1 DeserializeType<T1>(object data)
			{
				throw new NotImplementedException();
			}
		}

		private class DummyPathProvider1 : ISettingPathProvider
		{
			public string GetFullSettingsPath(string path, string fileName)
			{
				throw new NotImplementedException();
			}
		}
		
		private class DummyPathProvider2 : ISettingPathProvider
		{
			public string GetFullSettingsPath(string path, string fileName)
			{
				throw new NotImplementedException();
			}
		}
	}
}