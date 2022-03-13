using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.Settings.Tests.Editor
{
	public class SerializationTest
	{
		private ISettingSerializer serializer;
		private readonly Dictionary<string, object> settingsData = new Dictionary<string, object>();

		[SetUp]
		public void SetUp()
		{
			serializer = new SettingSerializer();
			settingsData.Clear();
		}

		[Test]
		public void SerializeFloatSetting()
		{
			SerializeTest<FloatSetting, float>(5f);
		}

		[Test]
		public void SerializeToggleSetting()
		{
			SerializeTest<ToggleSetting, bool>(true);
		}

		[Test]
		public void SerializeAudioSetting()
		{
			SerializeTest<AudioSetting, int>(50);
		}

		private void SerializeTest<TSetting, TValue>(TValue newValue) where TSetting : Setting<TValue> where TValue : IEquatable<TValue>
		{
			SettingsObject settings = ScriptableObject.CreateInstance<SettingsObject>();
			TSetting setting = ScriptableObject.CreateInstance<TSetting>();

			settings.Categories = new SettingsCategory[1]
			{
				new SettingsCategory()
				{
					Settings = new Setting[1]
					{
						setting
					}
				}
			};

			setting.Identifier = "setting";
			setting.Value = newValue;

			serializer.FillData(settings, settingsData);
			string json = serializer.SerializeToJson(settingsData);

			setting.Value = default;

			serializer.DeserializeFromJson(json, settingsData);

			Assert.IsTrue(settingsData.TryGetValue("setting", out object value));

			setting.SetSerializedValue(value);
			
			Assert.AreEqual(newValue, setting.Value);
		}
	}
}