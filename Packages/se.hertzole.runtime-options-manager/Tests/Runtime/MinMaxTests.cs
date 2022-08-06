using NUnit.Framework;
using UnityEditor;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class MinMaxTests : BaseTest
	{
		[Test]
		public void IntMinClamp([ValueSource(nameof(intValues))] int value)
		{
			IntSetting setting = AddSetting<IntSetting>();
			setting.MinValue = new ToggleableInt(true, 10);
			setting.Value = value;
			Assert.AreEqual(value < setting.MinValue ? setting.MinValue : value, setting.Value);
		}

		[Test]
		public void IntMaxClamp([ValueSource(nameof(intValues))] int value)
		{
			IntSetting setting = AddSetting<IntSetting>();
			setting.MaxValue = new ToggleableInt(true, 10);
			setting.Value = value;
			Assert.AreEqual(value > setting.MaxValue ? setting.MaxValue : value, setting.Value);
		}

		[Test]
		public void FloatMinClamp([ValueSource(nameof(floatValues))] float value)
		{
			FloatSetting setting = AddSetting<FloatSetting>();
			setting.MinValue = new ToggleableFloat(true, 10.0f);
			setting.Value = value;
			Assert.AreEqual(value < setting.MinValue ? setting.MinValue : value, setting.Value);
		}

		[Test]
		public void FloatMaxClamp([ValueSource(nameof(floatValues))] float value)
		{
			FloatSetting setting = AddSetting<FloatSetting>();
			setting.MaxValue = new ToggleableFloat(true, 10.0f);
			setting.Value = value;
			Assert.AreEqual(value > setting.MaxValue ? setting.MaxValue : value, setting.Value);
		}

		[Test]
		public void AudioMinClamp([ValueSource(nameof(intValues))] int value)
		{
			AudioSetting setting = AddSetting<AudioSetting>();
			setting.MinValue = new ToggleableInt(true, 10);
			setting.Value = value;
			Assert.AreEqual(value < setting.MinValue ? setting.MinValue : value, setting.Value);
		}
		
		[Test]
		public void AudioMaxClamp([ValueSource(nameof(intValues))] int value)
		{
			AudioSetting setting = AddSetting<AudioSetting>();
			setting.MaxValue = new ToggleableInt(true, 10);
			setting.Value = value;
			Assert.AreEqual(value > setting.MaxValue ? setting.MaxValue : value, setting.Value);
		}
	}
}