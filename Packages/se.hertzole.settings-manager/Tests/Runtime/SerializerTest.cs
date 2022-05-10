using System;
using System.Collections.Generic;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.SettingsManager.Tests
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
		public void SerializeFloatSetting()
		{
			SerializeTest<FloatSetting, float>(6.9f);
		}

		[Test]
		public void SerializeIntSetting()
		{
			SerializeTest<IntSetting, int>(42);
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

		private void SerializeTest<TSetting, TValue>(TValue newValue) where TSetting : Setting<TValue> where TValue : IEquatable<TValue>
		{
			TSetting setting = AddSetting<TSetting>();

			setting.Identifier = "setting";
			setting.Value = newValue;

			Dictionary<string, object> serializedData = new Dictionary<string, object>();

			settings.GetSerializeData(serializedData);

			byte[] savedData = serializer.Serialize(serializedData);

			setting.Value = default;

			serializedData.Clear();

			serializer.Deserialize(savedData, serializedData);

			Assert.IsTrue(serializedData.TryGetValue("setting", out object value));

			setting.SetSerializedValue(value, serializer);

			Assert.AreEqual(newValue, setting.Value);
		}

		private void SetInvalidSerializedValue<TSetting, TValue>() where TSetting : Setting<TValue>
		{
			TSetting setting = AddSetting<TSetting>();
			setting.SetSerializedValue(null, serializer);
			Assert.AreEqual(setting.DefaultValue, setting.Value);
		}
	}
}