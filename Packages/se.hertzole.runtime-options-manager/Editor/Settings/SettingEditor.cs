using UnityEditor;
using UnityEngine;

namespace Hertzole.OptionsManager.Editor
{
	[CustomEditor(typeof(Setting), true, isFallback = true)]
	public class SettingEditor : UnityEditor.Editor
	{
		// General Settings
		private SerializedProperty displayName;
#if HERTZ_SETTINGS_LOCALIZATION
		private SerializedProperty displayNameLocalized;
#endif
		private SerializedProperty identifier;

		// UI Settings
		private SerializedProperty uiPrefab;
#if HERTZ_SETTINGS_UIELEMENTS
		private SerializedProperty uiElement;
#endif
		
		// Saving settings
		private SerializedProperty overwriteSavePath;
		private SerializedProperty overriddenSavePath;
		private SerializedProperty overwriteFileName;
		private SerializedProperty overriddenFileName;

		private string[] propertyPathToExcludeForChildClasses;
		
		private static readonly GUIContent identifierContent = new GUIContent("Identifier", "The key string that will be used to identify this setting in the settings file.");
		private static readonly GUIContent uiPrefabContent = new GUIContent("UI Prefab", "The UI prefab to use for this setting.");
#if HERTZ_SETTINGS_UIELEMENTS
		private static readonly GUIContent uiElementContent = new GUIContent("UI Element", "The UI element to use for this setting.");
#endif

		protected virtual void OnEnable()
		{
			displayName = serializedObject.FindProperty(nameof(displayName));
#if HERTZ_SETTINGS_LOCALIZATION
			displayNameLocalized = serializedObject.FindProperty(nameof(displayNameLocalized));
#endif
			identifier = serializedObject.FindProperty(nameof(identifier));
			
			uiPrefab = serializedObject.FindProperty(nameof(uiPrefab));
#if HERTZ_SETTINGS_UIELEMENTS
			uiElement = serializedObject.FindProperty(nameof(uiElement));
#endif
			
			overwriteSavePath = serializedObject.FindProperty(nameof(overwriteSavePath));
			overriddenSavePath = serializedObject.FindProperty(nameof(overriddenSavePath));
			overwriteFileName = serializedObject.FindProperty(nameof(overwriteFileName));
			overriddenFileName = serializedObject.FindProperty(nameof(overriddenFileName));

			propertyPathToExcludeForChildClasses = new[]
			{
				serializedObject.FindProperty("m_Script").propertyPath,
				displayName.propertyPath,
#if HERTZ_SETTINGS_LOCALIZATION
				displayNameLocalized.propertyPath,
#endif
				identifier.propertyPath,
				uiPrefab.propertyPath,
#if HERTZ_SETTINGS_UIELEMENTS
				uiElement.propertyPath,
#endif
				overwriteSavePath.propertyPath,
				overriddenSavePath.propertyPath,
				overwriteFileName.propertyPath,
				overriddenFileName.propertyPath,
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(displayName);
#if HERTZ_SETTINGS_LOCALIZATION
			EditorGUILayout.PropertyField(displayNameLocalized);
#endif
			EditorGUILayout.PropertyField(identifier, identifierContent);

			DrawHeader("UI");
			EditorGUILayout.PropertyField(uiPrefab, uiPrefabContent);
#if HERTZ_SETTINGS_UIELEMENTS
			EditorGUILayout.PropertyField(uiElement, uiElementContent);
#endif
			
			DrawHeader("Saving");
			EditorGUILayout.PropertyField(overwriteSavePath);
			if (overwriteSavePath.boolValue)
			{
				EditorGUILayout.PropertyField(overriddenSavePath);
			}
			EditorGUILayout.PropertyField(overwriteFileName);
			if (overwriteFileName.boolValue)
			{
				EditorGUILayout.PropertyField(overriddenFileName);
			}


			DrawHeader("Value Settings");
			DrawChildClassProperties();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawChildClassProperties()
		{
			if(GetType() != typeof(SettingEditor))
			{
				return;
			}

			DrawPropertiesExcluding(serializedObject, propertyPathToExcludeForChildClasses);
		}

		protected static void DrawHeader(string text)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
		}
	}
}