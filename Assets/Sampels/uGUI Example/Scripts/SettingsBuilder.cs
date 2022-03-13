using Hertzole.Settings;
using UnityEngine;
using UnityEngine.UI;

public class SettingsBuilder : MonoBehaviour
{
	[SerializeField]
	private SettingsObject settings = default;

	[Space]
	[SerializeField]
	private Transform contentParent = default;
	[SerializeField]
	private Text categoryLabelPrefab = default;

	private void Start()
	{
		BuildSettingsUI();
	}

	private void BuildSettingsUI()
	{
		foreach (SettingsCategory category in settings.Categories)
		{
			BuildCategory(category);

			foreach (Setting setting in category.Settings)
			{
				BuildSetting(setting);
			}
		}
	}

	protected virtual void BuildCategory(SettingsCategory category)
	{
		Text text = Instantiate(categoryLabelPrefab, contentParent);
		text.text = category.DisplayName;
	}

	protected virtual void BuildSetting(Setting setting)
	{
		if (setting.UiPrefab == null)
		{
			Debug.LogError($"Can't create setting UI for {setting.name} as there is no UI prefab assigned.");
			return;
		}

		if (!setting.UiPrefab.TryGetComponent(out BaseSettingElement settingElement))
		{
			Debug.LogError($"Can't create setting UI for {setting.name} as the UI prefab is not of type BaseSettingElement.");
			return;
		}

		BaseSettingElement element = Instantiate(settingElement, contentParent);
		element.BindSetting(setting);
	}
}