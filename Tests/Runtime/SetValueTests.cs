using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class SetValueTests : BaseTest
	{
		[Test]
		public void SetValue_Float()
		{
			SetValueTest<FloatSetting, float>(6.9f);
		}

		[Test]
		public void SetValue_Int()
		{
			SetValueTest<IntSetting, int>(6);
		}

		[Test]
		public void SetValue_Bool()
		{
			SetValueTest<ToggleSetting, bool>(true);
		}

		[Test]
		public void SetValue_VSync()
		{
			QualitySettings.vSyncCount = 0;
			Assert.AreEqual(0, QualitySettings.vSyncCount);
			SetValueTest<VSyncSetting, bool>(true);
			Assert.AreEqual(1, QualitySettings.vSyncCount);
		}

		private void SetValueTest<TSetting, TValue>(TValue newValue) where TSetting : Setting<TValue>
		{
			TSetting setting = AddSetting<TSetting>();

			bool valueChanged = false;

			setting.OnValueChanged += value =>
			{
				Assert.AreEqual(value, newValue);
				valueChanged = true;
			};

			setting.Value = newValue;

			Assert.IsTrue(valueChanged);
		}
	}
}