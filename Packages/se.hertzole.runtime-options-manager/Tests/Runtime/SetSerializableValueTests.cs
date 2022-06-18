using NUnit.Framework;

namespace Hertzole.OptionsManager.Tests
{
	public class SetSerializableValueTests : BaseTest
	{
		[Test]
		public void SetBoolWithInt()
		{
			ToggleSetting toggleSetting = AddSetting<ToggleSetting>();
			toggleSetting.Value = false;
			toggleSetting.SetSerializedValue(1, settings.Serializer);
			Assert.IsTrue(toggleSetting.Value);
		}
	}
}