using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Hertzole.OptionsManager.Tests
{
	public class BaseTest
	{
		protected SettingsManager settings;
		protected readonly List<GameObject> objects = new List<GameObject>();
		
		protected static readonly bool[] boolValues = { true, false };
		protected static readonly int[] intValues = { int.MinValue, int.MaxValue, 0, -42, 42, -69, 69, -420, 420, -1, 1 };
		protected static readonly float[] floatValues = { float.MinValue, float.MaxValue, 0, -42.42f, 42.42f, -69.69f, 69.69f, -420.69f, 420.69f, -1.1f, 1.1f };

		[SetUp]
		public void Setup()
		{
			OnPreSetup();
			
			settings = ScriptableObject.CreateInstance<SettingsManager>();
			settings.name = "[TEST ONLY] Settings Manager";
			settings.LoadSettingsOnBoot = false;
			settings.AutoSaveSettings = false;
			settings.SaveLocation = SettingsManager.SaveLocations.DataPath;
			settings.SavePath = "test_settings";
			settings.FileName = "settings_test_file.json";
			settings.Categories.Add(new SettingsCategory());
			
			settings.Initialize();

			CleanUp();
			
			OnSetup();
		}

		[TearDown]
		public void TearDown()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				Object.DestroyImmediate(objects[i]);
			}
			
			objects.Clear();
			
			CleanUp();
		
			OnTearDown();
		}

		protected virtual void CleanUp()
		{
			string directory = Path.GetDirectoryName(settings.ComputedSavePath);
			if (Directory.Exists(directory))
			{
				Directory.Delete(directory, true);
			}
		}

		protected virtual void OnPreSetup() { }

		protected virtual void OnSetup() { }

		protected virtual void OnTearDown() { }

		protected T AddSetting<T>(bool hasValueLimits = false) where T : BaseSetting
		{
			T setting = ScriptableObject.CreateInstance<T>();
			settings.Categories[0].Settings.Add(setting);
			
			if (setting is IMinMaxInt minMaxInt)
			{
				minMaxInt.HasMaxValue = hasValueLimits;
				minMaxInt.HasMinValue = hasValueLimits;
			}

			if (setting is IMinMaxFloat minMaxFloat)
			{
				minMaxFloat.HasMaxValue = hasValueLimits;
				minMaxFloat.HasMinValue = hasValueLimits;
			}
			
			return setting;
		}
	}
}