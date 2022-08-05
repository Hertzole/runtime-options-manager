using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Hertzole.OptionsManager.Tests
{
	public class ResolutionTests : BaseTest
	{
		[Test]
		public void GetSet_DropdownValue()
		{
			ResolutionSetting setting = AddSetting<ResolutionSetting>();
			Resolution[] resolutions = setting.GetUniqueResolutions();
			for (int i = 0; i < resolutions.Length; i++)
			{
				setting.SetDropdownValue(i);

				Assert.AreEqual(resolutions[i].width, resolutions[setting.GetDropdownValue()].width);
				Assert.AreEqual(resolutions[i].height, resolutions[setting.GetDropdownValue()].height);
			}
		}

		[Test]
		public void Set_InvalidSerializableValue()
		{
			ResolutionSetting setting = AddSetting<ResolutionSetting>();
			setting.SetSerializedValue(42, new JsonSettingSerializer());
			Assert.AreEqual(Screen.currentResolution, setting.Value);
		}

		[Test]
		public void Get_DropdownValues()
		{
			ResolutionSetting setting = AddSetting<ResolutionSetting>();
			setting.ResolutionFormat = "{0} and {1}";
			var resolutions = setting.GetUniqueResolutions();
			var dropdownValues = setting.GetDropdownValues();
			Assert.AreEqual(resolutions.Length, dropdownValues.Count);

			for (int i = 0; i < dropdownValues.Count; i++)
			{
				Assert.AreEqual($"{resolutions[i].width} and {resolutions[i].height}", dropdownValues[i].text);
			}
		}
	}
}