using NUnit.Framework;

namespace Hertzole.SettingsManager.Tests
{
	public class MinMaxTests : BaseTest
	{
		[Test]
		public void IntMinClamp()
		{
			IntSetting setting = AddSetting<IntSetting>();
			setting.HasMinValue = true;
			setting.MinValue = 10;
			setting.Value = 5;
			Assert.AreEqual(10, setting.Value);
		}

		[Test]
		public void IntMaxClamp()
		{
			IntSetting setting = AddSetting<IntSetting>();
			setting.HasMaxValue = true;
			setting.MaxValue = 10;
			setting.Value = 15;
			Assert.AreEqual(10, setting.Value);
		}

		[Test]
		public void FloatMinClamp()
		{
			FloatSetting setting = AddSetting<FloatSetting>();
			setting.HasMinValue = true;
			setting.MinValue = 10.0f;
			setting.Value = 5.0f;
			Assert.AreEqual(10.0f, setting.Value);
		}

		[Test]
		public void FloatMaxClamp()
		{
			FloatSetting setting = AddSetting<FloatSetting>();
			setting.HasMaxValue = true;
			setting.MaxValue = 10.0f;
			setting.Value = 15.0f;
			Assert.AreEqual(10.0f, setting.Value);
		}

		[Test]
		public void AudioMinClamp()
		{
			AudioSetting setting = AddSetting<AudioSetting>();
			setting.HasMinValue = true;
			setting.MinValue = 10;
			setting.Value = 5;
			Assert.AreEqual(10, setting.Value);
		}
		
		[Test]
		public void AudioMaxClamp()
		{
			AudioSetting setting = AddSetting<AudioSetting>();
			setting.HasMaxValue = true;
			setting.MaxValue = 10;
			setting.Value = 15;
			Assert.AreEqual(10, setting.Value);
		}
	}
}