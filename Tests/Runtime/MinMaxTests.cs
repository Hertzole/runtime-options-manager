using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class MinMaxTests : BaseTest
	{
		[Test]
		public void IntMinClamp([ValueSource(nameof(intValues))] int value)
		{
			IntSetting setting = AddSetting<IntSetting>();
			setting.HasMinValue = true;
			setting.MinValue = 10;
			setting.Value = value;
			Assert.AreEqual(value < setting.MinValue ? setting.MinValue : value, setting.Value);
		}

		[Test]
		public void IntMaxClamp([ValueSource(nameof(intValues))] int value)
		{
			IntSetting setting = AddSetting<IntSetting>();
			setting.HasMaxValue = true;
			setting.MaxValue = 10;
			setting.Value = value;
			Assert.AreEqual(value > setting.MaxValue ? setting.MaxValue : value, setting.Value);
		}

		[Test]
		public void FloatMinClamp([ValueSource(nameof(floatValues))] float value)
		{
			FloatSetting setting = AddSetting<FloatSetting>();
			setting.HasMinValue = true;
			setting.MinValue = 10.0f;
			setting.Value = value;
			Assert.AreEqual(value < setting.MinValue ? setting.MinValue : value, setting.Value);
		}

		[Test]
		public void FloatMaxClamp([ValueSource(nameof(floatValues))] float value)
		{
			FloatSetting setting = AddSetting<FloatSetting>();
			setting.HasMaxValue = true;
			setting.MaxValue = 10.0f;
			setting.Value = value;
			Assert.AreEqual(value > setting.MaxValue ? setting.MaxValue : value, setting.Value);
		}

		[Test]
		public void AudioMinClamp([ValueSource(nameof(intValues))] int value)
		{
			AudioSetting setting = AddSetting<AudioSetting>();
			setting.HasMinValue = true;
			setting.MinValue = 10;
			setting.Value = value;
			Assert.AreEqual(value < setting.MinValue ? setting.MinValue : value, setting.Value);
		}
		
		[Test]
		public void AudioMaxClamp([ValueSource(nameof(intValues))] int value)
		{
			AudioSetting setting = AddSetting<AudioSetting>();
			setting.HasMaxValue = true;
			setting.MaxValue = 10;
			setting.Value = value;
			Assert.AreEqual(value > setting.MaxValue ? setting.MaxValue : value, setting.Value);
		}
	}
}