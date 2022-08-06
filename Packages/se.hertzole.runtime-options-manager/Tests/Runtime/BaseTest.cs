using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hertzole.OptionsManager.Tests
{
	public class BaseTest
	{
		protected SettingsManager settings;
		protected readonly List<GameObject> objects = new List<GameObject>();

		protected static readonly bool[] boolValues = { true, false };
		protected static readonly int[] intValues = { int.MinValue, int.MaxValue, 0, -42, 42, -69, 69, -420, 420, -1, 1 };
		protected static readonly float[] floatValues = { float.MinValue, float.MaxValue, 0, -42.42f, 42.42f, -69.69f, 69.69f, -420.69f, 420.69f, -1.1f, 1.1f };

		[UnitySetUp]
		public IEnumerator Setup()
		{
			OnPreSetup();

			settings = ScriptableObject.CreateInstance<SettingsManager>();
			settings.name = "[TEST ONLY] Settings Manager";
			settings.LoadSettingsOnBoot = false;
			settings.AutoSaveSettings = false;
			settings.SaveLocation = SettingsManager.SaveLocations.DataPath;
			settings.SavePath = "test_settings";
			settings.FileName = "settings_test_file.json";
			// Don't initialize because we call initialize right after.
			settings.AddCategory("Test Category", null, false);

			settings.Initialize();

			CleanUp();

			OnSetup();

			yield return OnSetUpRoutine();
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				Object.DestroyImmediate(objects[i]);
			}

			objects.Clear();

			CleanUp();

			OnTearDown();

			yield return OnTearDownRoutine();
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

		protected virtual IEnumerator OnSetUpRoutine()
		{
			yield return null;
		}

		protected virtual void OnTearDown() { }

		protected virtual IEnumerator OnTearDownRoutine()
		{
			yield return null;
		}

		protected virtual T AddSetting<T>(bool hasValueLimits = false) where T : BaseSetting
		{
			T setting = ScriptableObject.CreateInstance<T>();
			settings.Categories[0].AddSetting(setting);

			if (setting is IMinMaxInt minMaxInt)
			{
				ToggleableInt oldMinValue = minMaxInt.MinValue;
				ToggleableInt oldMaxValue = minMaxInt.MaxValue;
				ToggleableInt newMinValue = new ToggleableInt(hasValueLimits, oldMinValue.Value);
				ToggleableInt newMaxValue = new ToggleableInt(hasValueLimits, oldMaxValue.Value);
				minMaxInt.MinValue = newMinValue;
				minMaxInt.MaxValue = newMaxValue;
			}

			if (setting is IMinMaxFloat minMaxFloat)
			{
				ToggleableFloat oldMinValue = minMaxFloat.MinValue;
				ToggleableFloat oldMaxValue = minMaxFloat.MaxValue;
				ToggleableFloat newMinValue = new ToggleableFloat(hasValueLimits, oldMinValue.Value);
				ToggleableFloat newMaxValue = new ToggleableFloat(hasValueLimits, oldMaxValue.Value);
				minMaxFloat.MinValue = newMinValue;
				minMaxFloat.MaxValue = newMaxValue;
			}

			return setting;
		}
	}
}