using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class RefreshRateTests : BaseTest
	{
		[Test]
		public void SetValueUpdatesTargetFrameRate([ValueSource(nameof(intValues))] int value)
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			setting.Value = value;

			Assert.AreEqual(value, setting.Value);
			Assert.AreEqual(value, Application.targetFrameRate);
		}

		[Test]
		public void SetSerializableValue_String()
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			setting.SetSerializedValue("60", settings.Serializer);

			Assert.AreEqual(60, setting.Value);
			Assert.AreEqual(60, Application.targetFrameRate);
		}

		[Test]
		public void SetSerializableValue_StringInvalid()
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			setting.SetSerializedValue("hello", settings.Serializer);

			int screenRefreshRate = Screen.currentResolution.refreshRate;
			Debug.Log(screenRefreshRate + " " + setting.Value);

			Assert.AreEqual(screenRefreshRate, setting.Value);
			Assert.AreEqual(screenRefreshRate, Application.targetFrameRate);
		}

		[Test]
		public void SetSerializableValue_Invalid()
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			setting.SetSerializedValue(new Vector3(), settings.Serializer);

			int screenRefreshRate = Screen.currentResolution.refreshRate;

			Assert.AreEqual(screenRefreshRate, setting.Value);
			Assert.AreEqual(screenRefreshRate, Application.targetFrameRate);
		}

		[Test]
		public void SetSerializableValue_Null()
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			setting.SetSerializedValue(null, settings.Serializer);

			Assert.AreEqual(-1, setting.Value);
			Assert.AreEqual(-1, Application.targetFrameRate);
		}

		[Test]
		public void GetSet_DropdownValue()
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			int[] refreshRates = setting.GetUniqueRefreshRates();
			for (int i = 0; i < refreshRates.Length; i++)
			{
				setting.SetDropdownValue(i);

				Assert.AreEqual(refreshRates[i], refreshRates[setting.GetDropdownValue()]);
			}
		}

		[Test]
		public void Get_DropdownValues()
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			int[] refreshRates = setting.GetUniqueRefreshRates();
			IReadOnlyList<(string text, Sprite icon)> dropdownValues = setting.GetDropdownValues();
			Assert.AreEqual(refreshRates.Length, dropdownValues.Count);

			for (int i = 0; i < dropdownValues.Count; i++)
			{
				Assert.AreEqual(refreshRates[i].ToString(), dropdownValues[i].text);
			}
		}

		[Test]
		public void Get_DropdownValueInvalidValue()
		{
			RefreshRateSetting setting = AddSetting<RefreshRateSetting>();
			setting.Value = int.MinValue;
			int dropdownValue = setting.GetDropdownValue();

			Assert.AreEqual(0, dropdownValue);
		}
	}
}