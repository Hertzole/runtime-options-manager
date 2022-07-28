using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Hertzole.OptionsManager.Samples.UI
{
	public class SettingsBuilder : MonoBehaviour
	{
		[SerializeField]
		private SettingsManager settings = default;

		[Space]
		[SerializeField]
		private Transform contentParent = default;
		[SerializeField]
		private Text categoryLabelPrefab = default;

		private void Awake()
		{
			// If no settings have been assigned, use the global one.
			if (settings == null)
			{
				settings = SettingsManager.Instance;
			}
			else
			{
				// When you have custom settings, it's important to initialize them.
				settings.Initialize();
			}

			BuildSettingsUI();
		}

		private void BuildSettingsUI()
		{
			foreach (SettingsCategory category in settings.Categories)
			{
				BuildCategory(category);

				foreach (BaseSetting baseSetting in category.Settings)
				{
					if (baseSetting is Setting setting)
					{
						BuildSetting(setting);
					}
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
}