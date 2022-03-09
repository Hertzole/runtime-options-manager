using Hertzole.Settings;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsUiToolkit : MonoBehaviour
{
	[SerializeField]
	private SettingsObject settings = default;
	[SerializeField]
	private UIDocument doc = default;

	[Header("UI Elements")]
	[SerializeField]
	private VisualTreeAsset categoryHeader = default;

	private void Start()
	{
		BuildUi();
	}

	private void BuildUi()
	{
		VisualElement root = doc.rootVisualElement.Q<VisualElement>("settings-root");

		for (int i = 0; i < settings.Categories.Length; i++)
		{
			TemplateContainer headerLabel = categoryHeader.Instantiate();
			headerLabel.Q<Label>().text = settings.Categories[i].DisplayName;

			root.Add(headerLabel);

			for (int j = 0; j < settings.Categories[i].Settings.Length; j++)
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