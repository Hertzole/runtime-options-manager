using UnityEngine;
using UnityEngine.UI;

namespace Hertzole.OptionsManager.Samples.UI
{
	public class ToggleSetting : BaseSettingElement
	{
		[SerializeField]
		private Toggle toggle = default;

		protected override void OnBindSetting(Setting setting)
		{
			if (!(setting is Setting<bool> toggleSetting))
			{
				Debug.LogError("ToggleSetting only supports settings of type Setting<bool>.");
				return;
			}

			toggle.SetIsOnWithoutNotify(toggleSetting.Value);
			toggle.onValueChanged.AddListener(x => toggleSetting.Value = x);
		}
	}
}