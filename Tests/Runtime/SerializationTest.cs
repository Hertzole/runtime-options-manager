using NUnit.Framework;

namespace Hertzole.SettingsManager.Tests
{
	public class SerializationTest : BaseTest
	{
		[Test]
		public void GetSerializeValue_IntSetting()
		{
			TestSerializeValue<IntSetting, int>(1);
		}
		
		[Test]
		public void GetSerializeValue_FloatSetting()
		{
			TestSerializeValue<FloatSetting, float>(1.1f);
		}
		
		[Test]
		public void GetSerializeValue_BoolSetting()
		{
			TestSerializeValue<ToggleSetting, bool>(true);
		}
		
		[Test]
		public void GetSerializeValue_AudioSetting()
		{
			TestSerializeValue<AudioSetting, int>(1);
		}
		
		private void TestSerializeValue<TSetting, TValue>(TValue newValue) where TSetting : Setting<TValue>, new()
		{
			TSetting setting = AddSetting<TSetting>();
			setting.Value = newValue;
			setting.GetSerializeValue();

			Assert.AreEqual(newValue, setting.Value);
		}
	}
}