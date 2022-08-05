#if HERTZ_SETTINGS_LOCALIZATION
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Localization;

namespace Hertzole.OptionsManager.Editor
{
	[CustomEditor(typeof(LanguageSetting))]
	public class LanguageSettingEditor : SettingEditor
	{
		private SerializedProperty defaultValue;
		private SerializedProperty nameDisplayType;
		private SerializedProperty customNames;
		
		private ReorderableList customNamesList;

		private LanguageSetting setting;

		private static GUIContent menuIcon;

		private static readonly GUIContent getInfoNameContent = new GUIContent("Get Info Name");
		private static readonly GUIContent getDisplayNameContent = new GUIContent("Get Display Name");
		private static readonly GUIContent getNativeNameContent = new GUIContent("Get Native Name");
		private static readonly GUIContent getEnglishNameContent = new GUIContent("Get English Name");

		protected override void OnEnable()
		{
			base.OnEnable();

			defaultValue = serializedObject.FindProperty(nameof(defaultValue));
			nameDisplayType = serializedObject.FindProperty(nameof(nameDisplayType));
			customNames = serializedObject.FindProperty(nameof(customNames));
			
			customNamesList = new ReorderableList(serializedObject, customNames, true, true, true, true)
			{
				drawHeaderCallback = DrawCustomNamesListHeader,
				drawElementCallback = DrawCustomNamesListElement
			};
			
			setting = (LanguageSetting)target;

		}

		private void DrawCustomNamesListHeader(Rect rect)
		{
			var r = new Rect(rect.x + 16, rect.y, (rect.width / 2) - 2, rect.height);

			// Draw two headers with even spacing, one for locale and one for name.
			EditorGUI.LabelField(r, "Locale");
			r.x += r.width - 4;
			EditorGUI.LabelField(r, "Name");
		}

		private void DrawCustomNamesListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var element = customNames.GetArrayElementAtIndex(index);
			var key = element.FindPropertyRelative("key");
			var value = element.FindPropertyRelative("value");
			
			var r = new Rect(rect.x, rect.y + 2, (rect.width / 2) - 16, EditorGUIUtility.singleLineHeight);
			
			// Draw two fields with even spacing, one for locale and one for name.
			EditorGUI.BeginChangeCheck();
			EditorGUI.PropertyField(r, key, GUIContent.none);
			if (EditorGUI.EndChangeCheck() && key.objectReferenceValue is Locale locale)
			{
				value.stringValue = locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.NativeName : locale.Identifier.ToString();
			}
			
			r.x += r.width + 4;
			EditorGUI.PropertyField(r, value, GUIContent.none);
			
			r.x += r.width + 4;
			r.width = 24;

			var oEnabled = GUI.enabled;
			GUI.enabled = key.objectReferenceValue != null;
			if (GUI.Button(r, menuIcon))
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(getInfoNameContent, false, _ =>
				{
					value.stringValue = setting.GetLocaleName(key.objectReferenceValue as Locale, LanguageSetting.DisplayType.CultureInfoName);
					serializedObject.ApplyModifiedProperties();
				}, null);
				menu.AddItem(getDisplayNameContent, false, _ =>
				{
					value.stringValue = setting.GetLocaleName(key.objectReferenceValue as Locale, LanguageSetting.DisplayType.CultureInfoDisplayName);
					serializedObject.ApplyModifiedProperties();
				}, null);
				menu.AddItem(getNativeNameContent, false, _ =>
				{
					value.stringValue = setting.GetLocaleName(key.objectReferenceValue as Locale, LanguageSetting.DisplayType.CultureInfoNativeName);
					serializedObject.ApplyModifiedProperties();
				}, null);
				menu.AddItem(getEnglishNameContent, false, _ =>
				{
					value.stringValue = setting.GetLocaleName(key.objectReferenceValue as Locale, LanguageSetting.DisplayType.CultureInfoEnglishName);
					serializedObject.ApplyModifiedProperties();
				}, null);

				menu.ShowAsContext();
			}
			GUI.enabled = oEnabled;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			menuIcon ??=  EditorGUIUtility.TrIconContent("_Menu");

			serializedObject.Update();
			
			EditorGUILayout.PropertyField(defaultValue);
			EditorGUILayout.PropertyField(nameDisplayType);

			if (nameDisplayType.enumValueIndex == (int) LanguageSetting.DisplayType.CustomName)
			{
				EditorGUILayout.Space();
			
				customNamesList.DoLayoutList();
			}

			EditorGUILayout.Space();
			
			DoLanguageNamePreview();

			serializedObject.ApplyModifiedProperties();
		}

		private void DoLanguageNamePreview()
		{
			if (!(defaultValue.objectReferenceValue is Locale locale))
			{
				return;
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Language Name Preview:");
			EditorGUILayout.LabelField(setting.GetLocaleName(locale, (LanguageSetting.DisplayType) nameDisplayType.enumValueIndex));
			EditorGUILayout.EndHorizontal();
		}
	}
}
#endif