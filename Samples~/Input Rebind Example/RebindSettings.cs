using UnityEngine;
using UnityEngine.InputSystem;

namespace Hertzole.OptionsManager.Samples.InputRebinding
{
	public class RebindSettings : MonoBehaviour
	{
		[SerializeField]
		private SettingsManager settings = default;
		[SerializeField]
		private PlayerInput playerInput = default;
		[SerializeField]
		private RectTransform settingsContent = default;
		[SerializeField]
		private InputHelper inputHelper = default;

		private void Start()
		{
			// If no settings are assigned, use the global one.
			if (settings == null)
			{
				settings = SettingsManager.Instance;
			}
			else
			{
				// When you have custom settings, it's important to initialize them.
				settings.Initialize();
			}

			CreateSettings();
		}

		private void OnEnable()
		{
			inputHelper.OnResetControlsRequested += OnResetControls;
		}

		private void OnDisable()
		{
			inputHelper.OnResetControlsRequested -= OnResetControls;
		}

		private void OnResetControls()
		{
			foreach (SettingsCategory category in settings.Categories)
			{
				foreach (BaseSetting baseSetting in category.Settings)
				{
					if (baseSetting is InputSetting inputSetting)
					{
						inputSetting.ResetToDefault(playerInput);
					}
				}
			}
		}

		private void CreateSettings()
		{
			foreach (SettingsCategory category in settings.Categories)
			{
				foreach (BaseSetting baseSetting in category.Settings)
				{
					if (baseSetting is InputSetting inputSetting)
					{
						GameObject ui = Instantiate(inputSetting.UiPrefab, settingsContent);
						ui.GetComponent<InputSettingElement>().BindSetting(playerInput, inputSetting);
					}
					else if (baseSetting is ButtonSetting buttonSetting)
					{
						GameObject ui = Instantiate(buttonSetting.UiPrefab, settingsContent);
						ui.GetComponent<ButtonSettingElement>().BindSetting(buttonSetting);
					}
				}
			}
		}
	}
}