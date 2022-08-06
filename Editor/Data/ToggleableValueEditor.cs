using UnityEditor;
using UnityEngine;

namespace Hertzole.OptionsManager.Editor
{
	public abstract class ToggleableValueEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Rect controlRect = EditorGUI.PrefixLabel(position, label);
			
			SerializedProperty enabled = property.FindPropertyRelative("enabled");

			Rect r = controlRect;
			EditorGUI.PropertyField(r, enabled, GUIContent.none);
			
			if (enabled.boolValue)
			{
				r.x += 18;
				r.width -= 18;
				EditorGUI.PropertyField(r, property.FindPropertyRelative("value"), GUIContent.none);
			}
		}
	}
}