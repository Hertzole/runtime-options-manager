using Hertzole.SettingsManager;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsUiToolkit : MonoBehaviour
{
	[SerializeField]
	private UIDocument doc = default;

	[Header("UI Elements")]
	[SerializeField]
	private VisualTreeAsset categoryHeader = default;

	private void Start()
	{
		BuildUi();

		SettingsManager.Instance.SaveSettings();
	}

	private void BuildUi()
	{
		VisualElement root = doc.rootVisualElement.Q<VisualElement>("settings-root");

		SettingsManager settings = SettingsManager.Instance;

		for (int i = 0; i < settings.Categories.Count; i++)
		{
			TemplateContainer headerLabel = categoryHeader.Instantiate();
			headerLabel.Q<Label>().text = settings.Categories[i].DisplayName;
		
			root.Add(headerLabel);
		
			for (int j = 0; j < settings.Categories[i].Settings.Count; j++)
			{
				VisualElement uiElement = settings.Categories[i].Settings[j].CreateUIElement();
				if (uiElement == null)
				{
					continue;
				}
		
				root.Add(uiElement);
			}
		}
	}
}