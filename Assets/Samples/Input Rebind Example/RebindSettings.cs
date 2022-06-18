using UnityEngine;
using UnityEngine.InputSystem;

namespace Hertzole.RuntimeOptionsManager.Samples.InputRebinding
{
	public class RebindSettings : MonoBehaviour
	{
		[SerializeField]
		private SettingsManager settings = default;
		[SerializeField]
		private PlayerInput playerInput = default;
		[SerializeField]
		private RectTransform settingsContent = default;

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

		private void CreateSettings()
		{
			for (int i = 0; i < settings.Categories.Count; i++)
			{
				for (int j = 0; j < settings.Categories[i].Settings.Count; j++)
				{
					if (!(settings.Categories[i].Settings[j] is InputSetting inputSetting))
					{
						continue;
					}

					GameObject ui = Instantiate(inputSetting.UiPrefab, settingsContent);
					ui.GetComponent<InputSettingElement>().BindSetting(playerInput, inputSetting);
				}
			}
		}
	}
}