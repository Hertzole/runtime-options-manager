using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Hertzole.SettingsManager.Tests
{
	public class BaseTest
	{
		protected SettingsManager settings;
		protected readonly List<GameObject> objects = new List<GameObject>();

		[SetUp]
		public void Setup()
		{
			settings = ScriptableObject.CreateInstance<SettingsManager>();
			settings.LoadSettingsOnBoot = false;
			settings.AutoSaveSettings = false;
			settings.Categories.Add(new SettingsCategory());
			
			settings.Initialize();
			
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
		
			OnTearDown();
		}
		
		protected virtual void OnSetup() { }

		protected virtual void OnTearDown() { }

		protected T AddSetting<T>() where T : Setting
		{
			T setting = ScriptableObject.CreateInstance<T>();
			settings.Categories[0].Settings.Add(setting);
			return setting;
		}
	}
}