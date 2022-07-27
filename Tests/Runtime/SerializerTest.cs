using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public abstract partial class SerializerTest<T> : BaseTest where T : ISettingSerializer, new()
	{
		protected T serializer;
		
		protected override void OnSetup()
		{
			serializer = new T();
#if HERTZ_SETTINGS_INPUTSYSTEM
			InputSetup();
#endif
		}

		protected override void OnTearDown()
		{
#if HERTZ_SETTINGS_INPUTSYSTEM
			InputTearDown();
#endif
		}

		[Test]
		public void SerializeFloatSetting([ValueSource(nameof(floatValues))] float value)
		{
			SerializeTest<FloatSetting, float>(value);
		}

		[Test]
		public void SerializeIntSetting([ValueSource(nameof(intValues))] int value)
		{
			SerializeTest<IntSetting, int>(value);
		}

		[Test]
		public void SerializeToggleSetting([ValueSource(nameof(boolValues))] bool value)
		{
			SerializeTest<ToggleSetting, bool>(value);
		}

		[Test]
		public void SerializeAudioSetting([ValueSource(nameof(intValues))] int value)
		{
			SerializeTest<AudioSetting, int>(value);
		}
		
		[Test]
		public void SerializeVSyncOn()
		{
			QualitySettings.vSyncCount = 0;
			Assert.AreEqual(0, QualitySettings.vSyncCount);
			SerializeTest<VSyncSetting, bool>(true, setting =>
			{
				QualitySettings.vSyncCount = 0;
			});
			Assert.AreEqual(1, QualitySettings.vSyncCount);
		}
		
		[Test]
		public void SerializeVSyncOff()
		{
			QualitySettings.vSyncCount = 1;
			Assert.AreEqual(1, QualitySettings.vSyncCount);
			SerializeTest<VSyncSetting, bool>(false, setting =>
			{
				QualitySettings.vSyncCount = 1;
			});
			Assert.AreEqual(0, QualitySettings.vSyncCount);
		}

		[Test]
		public void SerializeNull()
		{
			byte[] data = serializer.Serialize(null);
			Assert.IsNotNull(data);
			Assert.AreEqual(0, data.Length);
		}

		[Test]
		public void DeserializeNull()
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			serializer.Deserialize(null, data);
			Assert.AreEqual(0, data.Count);
		}

		[Test]
		public void DeserializeEmptyArray()
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			serializer.Deserialize(Array.Empty<byte>(), data);
			Assert.AreEqual(0, data.Count);
		}

		[Test]
		public void SetInvalidSerializedValue_IntSetting()
		{
			SetInvalidSerializedValue<IntSetting, int>();
		}

		[Test]
		public void SetInvalidSerializedValue_FloatSetting()
		{
			SetInvalidSerializedValue<FloatSetting, float>();
		}

		[Test]
		public void SetInvalidSerializedValue_ToggleSetting()
		{
			SetInvalidSerializedValue<ToggleSetting, bool>();
		}

		[Test]
		public void SetInvalidSerializedValue_AudioSetting()
		{
			SetInvalidSerializedValue<AudioSetting, int>();
		}

		private void SerializeTest<TSetting, TValue>(TValue newValue, Action<TSetting> beforeSave = null) where TSetting : Setting<TValue> where TValue : IEquatable<TValue>
		{
			TSetting setting = AddSetting<TSetting>(false);

			setting.Identifier = "setting";
			setting.Value = newValue;

			beforeSave?.Invoke(setting);

			Dictionary<string, object> serializedData = new Dictionary<string, object>();

			settings.GetSerializeData(serializedData, new Setting[] {setting});

			byte[] savedData = serializer.Serialize(serializedData);

			setting.Value = default;

			serializedData.Clear();

			serializer.Deserialize(savedData, serializedData);

			Assert.IsTrue(serializedData.TryGetValue("setting", out object value));

			setting.SetSerializedValue(value, serializer);

			Assert.AreEqual(newValue, setting.Value, $"Value is {setting.Value} but should be {newValue} after deserialization.");
		}

		private void SetInvalidSerializedValue<TSetting, TValue>() where TSetting : Setting<TValue>
		{
			TSetting setting = AddSetting<TSetting>();
			setting.SetSerializedValue(null, serializer);
			Assert.AreEqual(setting.DefaultValue, setting.Value);
		}
	}
}