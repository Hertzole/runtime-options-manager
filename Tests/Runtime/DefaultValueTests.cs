using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class DefaultValueTests : BaseTest
	{
		[Test]
		public void GetDefaultValue_Int()
		{
			GetDefaultValue<int, IntSetting>(42);
		}
		
		[Test]
		public void GetDefaultValue_Float()
		{
			GetDefaultValue<float, FloatSetting>(42.0f);
		}
		
		[Test]
		public void GetDefaultValue_Bool()
		{
			GetDefaultValue<bool, ToggleSetting>(true);
		}
		
		private void GetDefaultValue<TValue, TSetting>(TValue defaultValue) where TSetting : Setting<TValue>
		{
			TSetting setting = AddSetting<TSetting>();
			setting.DefaultValue = defaultValue;

			Assert.AreEqual(defaultValue, setting.GetDefaultValue());
		}
	}
}