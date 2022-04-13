using Hertzole.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSettings : MonoBehaviour
{
	[SerializeField] 
	private PlayerInput playerInput = default;
	[SerializeField]
	private SettingsObject settings = default;
	[SerializeField]
	private RectTransform settingsContent = default;

	private void Start()
	{
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