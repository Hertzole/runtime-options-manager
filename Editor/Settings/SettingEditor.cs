using UnityEditor;

namespace Hertzole.Settings.Editor
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

			serializedObject.ApplyModifiedProperties();
		}
	}
}