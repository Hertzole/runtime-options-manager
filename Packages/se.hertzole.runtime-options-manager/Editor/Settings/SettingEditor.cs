using UnityEditor;

namespace Hertzole.OptionsManager.Editor
{
	[CustomEditor(typeof(Setting))]
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
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(displayName);
#if HERTZ_SETTINGS_LOCALIZATION
			EditorGUILayout.PropertyField(displayNameLocalized);
#endif
			EditorGUILayout.PropertyField(identifier);
			
			EditorGUILayout.PropertyField(uiPrefab);
#if HERTZ_SETTINGS_UIELEMENTS
			EditorGUILayout.PropertyField(uiElement);
#endif
			
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
			
			EditorGUILayout.Space();

			serializedObject.ApplyModifiedProperties();
		}
	}
}