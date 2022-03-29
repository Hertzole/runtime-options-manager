using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace Hertzole.Settings.Editor
{
	[CustomEditor(typeof(AudioSetting))]
	public class AudioSettingEditor : SettingEditor
	{
		// Value Settings
		private SerializedProperty defaultValue;
		private SerializedProperty hasMinValue;
		private SerializedProperty minValue;
		private SerializedProperty hasMaxValue;
		private SerializedProperty maxValue;
		private SerializedProperty enableSlider;

		private SerializedProperty targetAudioMixer;
		private SerializedProperty targetProperty;

		private string[] exposedParameters;
		private int selectedParameter;

		protected override void OnEnable()
		{
			base.OnEnable();
			
			defaultValue = serializedObject.FindProperty(nameof(defaultValue));
			hasMinValue = serializedObject.FindProperty(nameof(hasMinValue));
			minValue = serializedObject.FindProperty(nameof(minValue));
			hasMaxValue = serializedObject.FindProperty(nameof(hasMaxValue));
			maxValue = serializedObject.FindProperty(nameof(maxValue));
			enableSlider = serializedObject.FindProperty(nameof(enableSlider));
			
			targetAudioMixer = serializedObject.FindProperty(nameof(targetAudioMixer));
			targetProperty = serializedObject.FindProperty(nameof(targetProperty));
			
			GetExposedProperties();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			EditorGUILayout.PropertyField(defaultValue);
			EditorGUILayout.PropertyField(hasMinValue);
			if (hasMinValue.boolValue)
			{
				EditorGUILayout.PropertyField(minValue);
			}
			EditorGUILayout.PropertyField(hasMaxValue);
			if (hasMaxValue.boolValue)
			{
				EditorGUILayout.PropertyField(maxValue);
			}
			EditorGUILayout.PropertyField(enableSlider);

			EditorGUILayout.Space();
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(targetAudioMixer);
			if (EditorGUI.EndChangeCheck())
			{
				GetExposedProperties();
			}

			EditorGUI.BeginChangeCheck();
			selectedParameter = EditorGUILayout.Popup(targetProperty.displayName, selectedParameter, exposedParameters);
			if (EditorGUI.EndChangeCheck())
			{
				targetProperty.stringValue = selectedParameter == 0 ? string.Empty : exposedParameters[selectedParameter];
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void GetExposedProperties()
		{
			if (targetAudioMixer.objectReferenceValue == null)
			{
				selectedParameter = 0;
				exposedParameters = new string[] { "None" };
				return;
			}

			AudioMixer mixer = (AudioMixer) targetAudioMixer.objectReferenceValue;

			Array parameters = (Array) mixer.GetType()!.GetProperty("exposedParameters")!.GetValue(mixer, null);

			exposedParameters = new string[parameters.Length + 1];

			exposedParameters[0] = "None";
			
			for (int i = 0; i < parameters.Length; i++)
			{
				object o = parameters.GetValue(i);
				string param = (string) o.GetType().GetField("name").GetValue(o);
				exposedParameters[i + 1] = param;

				if (param == targetProperty.stringValue)
				{
					selectedParameter = i + 1;
				}
			}

			if (selectedParameter == 0)
			{
				targetProperty.stringValue = string.Empty;
			}

			serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
	}
}