using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class ButtonSettingTests : BaseTest
	{
		[Test]
		public void OnClickedInvokes()
		{
			ButtonSetting setting = AddSetting<ButtonSetting>();
			bool clicked = false;
			setting.OnClick.AddListener(() => clicked = true);
			setting.OnClick.Invoke();
			Assert.IsTrue(clicked);
		}
	}
}