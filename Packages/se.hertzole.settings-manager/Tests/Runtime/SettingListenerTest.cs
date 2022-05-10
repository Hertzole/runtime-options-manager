using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hertzole.SettingsManager.Tests
{
	public class SettingListenerTests : BaseTest
	{
		private GameObject go;

		protected override void OnSetup()
		{
			go = new GameObject("Listener");
			objects.Add(go);
		}

		[Test]
		public void OnValueChanged_Int()
		{
			TestListener<IntSettingListener, IntSetting, int>(42, false);
		}

		[Test]
		public void OnValueChanged_Float()
		{
			TestListener<FloatSettingListener, FloatSetting, float>(42.0f, false);
		}

		[Test]
		public void OnValueChanged_Bool()
		{
			TestListener<ToggleSettingListener, ToggleSetting, bool>(true, false);
		}

		[Test]
		public void OnValueChanging_Int()
		{
			TestListener<IntSettingListener, IntSetting, int>(42, true);
		}

		[Test]
		public void OnValueChanging_Float()
		{
			TestListener<FloatSettingListener, FloatSetting, float>(42.0f, true);
		}

		[Test]
		public void OnValueChanging_Bool()
		{
			TestListener<ToggleSettingListener, ToggleSetting, bool>(true, true);
		}

		[Test]
		public void SetSettingSubscribeUnsubscribe_OnValueChanged()
		{
			TestSubscribeEvents(false);
		}

		[Test]
		public void SetSettingSubscribeUnsubscribe_OnValueChanging()
		{
			TestSubscribeEvents(true);
		}

		[UnityTest]
		public IEnumerator InvokeOnStart()
		{
			IntSettingListener prefab = go.AddComponent<IntSettingListener>();
			IntSetting setting = AddSetting<IntSetting>();
			prefab.Setting = setting;

			bool called = false;

			IntSettingListener instance = Object.Instantiate(prefab);
			instance.OnValueChanged.AddListener(_ => { called = true; });

			objects.Add(instance.gameObject);

			yield return null;

			Assert.IsTrue(called);
		}

		[UnityTest]
		public IEnumerator InvokeOnStartWithValue()
		{
			IntSettingListener prefab = go.AddComponent<IntSettingListener>();
			IntSetting setting = AddSetting<IntSetting>();
			prefab.Setting = setting;

			bool called = false;

			setting.Value = 42;

			IntSettingListener instance = Object.Instantiate(prefab);
			instance.OnValueChanged.AddListener(value =>
			{
				Assert.AreEqual(42, value);
				called = true;
			});

			objects.Add(instance.gameObject);

			yield return null;

			Assert.IsTrue(called);
		}

		[Test]
		public void SettingGet()
		{
			IntSettingListener listener = go.AddComponent<IntSettingListener>();
			IntSetting setting = AddSetting<IntSetting>();

			UnityEngine.Assertions.Assert.IsNull(listener.Setting);
			listener.Setting = setting;
			UnityEngine.Assertions.Assert.AreEqual(setting, listener.Setting);
		}

		private void TestListener<TListener, TSetting, TValue>(TValue newValue, bool changing) where TListener : SettingListener<TValue, TSetting> where TSetting : Setting<TValue>
		{
			TListener listener = go.AddComponent<TListener>();

			TSetting setting = AddSetting<TSetting>();
			listener.Setting = setting;

			bool called = false;

			if (changing)
			{
				TValue previousValue = setting.Value;

				listener.OnValueChanging.AddListener(value =>
				{
					Assert.AreEqual(previousValue, value);
					called = true;
				});
			}
			else
			{
				listener.OnValueChanged.AddListener(value =>
				{
					Assert.AreEqual(newValue, value);
					called = true;
				});
			}

			setting.Value = newValue;

			Assert.IsTrue(called);
		}

		private void TestSubscribeEvents(bool changing)
		{
			IntSetting setting1 = AddSetting<IntSetting>();
			IntSetting setting2 = AddSetting<IntSetting>();

			IntSettingListener listener = go.AddComponent<IntSettingListener>();

			bool called = false;

			if (changing)
			{
				listener.OnValueChanging.AddListener(_ => called = true);
			}
			else
			{
				listener.OnValueChanged.AddListener(_ => called = true);
			}

			listener.Setting = setting1;
			setting1.Value = 42;

			Assert.IsTrue(called);
			called = false;

			listener.Setting = setting2;
			setting2.Value = 42;

			Assert.IsTrue(called);
			called = false;

			listener.Setting = null;

			setting1.Value = 100;
			setting2.Value = 100;

			Assert.IsFalse(called);
		}
	}
}