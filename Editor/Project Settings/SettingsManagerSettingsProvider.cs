using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hertzole.RuntimeOptionsManager.Editor
{
	public class SettingsManagerSettingsProvider : AssetSettingsProvider
	{
		private ObjectField settingsField;
		private InspectorElement editor;
		private VisualElement emptySettings;

		public SettingsManagerSettingsProvider() : base("Hertzole/Settings Manager", () => SettingsManagerEditorSettings.ActiveSettings) { }

		public override void OnActivate(string searchContext, VisualElement rootElement)
		{
			rootElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/se.hertzole.settingsmanager/Editor/Templates/ProjectSettings.uss"));
			
			ScrollView root = new ScrollView
			{ 
				style =
				{
					marginLeft = 9,
					marginTop = 1
				},
				horizontalScrollerVisibility = ScrollerVisibility.Hidden
			};

			root.Add(new Label("Settings Manager") { style = { marginBottom = 12, fontSize = 19, unityFontStyleAndWeight = FontStyle.Bold } });
			
			settingsField = new ObjectField("Settings")
			{
				objectType = typeof(SettingsManager),
				allowSceneObjects = false
			};

			settingsField.SetValueWithoutNotify(SettingsManagerEditorSettings.ActiveSettings);
			settingsField.RegisterValueChangedCallback(ctx =>
			{
				if (editor != null)
				{
					editor.RemoveFromHierarchy();
					editor = null;
				}
				
				if (ctx.newValue is SettingsManager settings)
				{
					SettingsManagerEditorSettings.ActiveSettings = settings;
					if (emptySettings != null)
					{
						emptySettings.RemoveFromHierarchy();
						emptySettings = null;
					}

					editor = CreateEditor(settings);
					root.Add(editor);
				}
				else
				{
					SettingsManagerEditorSettings.ActiveSettings = null;
					emptySettings = CreateEmptySettingsUI();
					root.Add(emptySettings);
				}
			});

			VisualElement space = new VisualElement
				{ style = { height = 4 } };

			root.Add(settingsField);
			root.Add(space);

			if (SettingsManagerEditorSettings.ActiveSettings != null)
			{
				editor = CreateEditor(SettingsManagerEditorSettings.ActiveSettings);
				
				root.Add(editor);
			}
			else
			{
				emptySettings = CreateEmptySettingsUI();
				root.Add(emptySettings);
			}

			rootElement.Add(root);
		}

		private static InspectorElement CreateEditor(SettingsManager settings)
		{
			InspectorElement element = new InspectorElement(settings)
			{
				style =
				{
					paddingLeft = 0
				}
			};

			return element;
		}

		private VisualElement CreateEmptySettingsUI()
		{
			VisualElement root = new VisualElement();

			HelpBox helpBox = new HelpBox("You have no active settings. Please create one.", HelpBoxMessageType.Info);
			Button createButton = new Button(ClickCreateSettings)
			{
				style = { width = 100 },
				text = "Create"
			};
			
			root.Add(helpBox);
			root.Add(createButton);

			return root;
		}

		private void ClickCreateSettings()
		{
			SettingsManager settings = SettingsManagerEditorSettings.CreateSettings();
			if (settings != null)
			{
				SettingsManagerEditorSettings.ActiveSettings = settings;
				if (settingsField != null)
				{
					settingsField.value = settings;
				}
			}
		}

		[SettingsProvider]
		private static SettingsProvider CreateProjectSettingsProvider()
		{
			return new SettingsManagerSettingsProvider();
		}
	}
}