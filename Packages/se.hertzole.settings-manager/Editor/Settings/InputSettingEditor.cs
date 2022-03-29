#if HERTZ_SETTINGS_INPUTSYSTEM
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Hertzole.Settings.Editor
{
	[CustomEditor(typeof(InputSetting))]
	public class InputSettingEditor : SettingEditor
	{
		private readonly GUIContent bindingLabel = new GUIContent("Binding");

		private GUIContent[] m_BindingOptions;

		private int m_SelectedBindingOption;
		private SerializedProperty bindings;
		private SerializedProperty targetAction;

		private string[] m_BindingOptionValues;

		protected override void OnEnable()
		{
			base.OnEnable();

			targetAction = serializedObject.FindProperty(nameof(targetAction));
			bindings = serializedObject.FindProperty(nameof(bindings));

			// RefreshBindingOptions();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(targetAction);

			EditorGUILayout.PropertyField(bindings, true);
			
			// int newSelectedBinding = EditorGUILayout.Popup(bindingLabel, m_SelectedBindingOption, m_BindingOptions);
			// if (newSelectedBinding != m_SelectedBindingOption)
			// {
			// 	string id = m_BindingOptionValues[newSelectedBinding];
			// 	bindings.stringValue = id;
			// 	m_SelectedBindingOption = newSelectedBinding;
			// }

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				// RefreshBindingOptions();
			}
		}

		protected void RefreshBindingOptions()
		{
			if (targetAction.objectReferenceValue == null)
			{
				m_BindingOptions = Array.Empty<GUIContent>();
				m_BindingOptionValues = Array.Empty<string>();
				m_SelectedBindingOption = -1;
				return;
			}
			
			InputActionReference actionReference = (InputActionReference) targetAction.objectReferenceValue;
			InputAction action = actionReference.action;

			if (action == null)
			{
				m_BindingOptions = Array.Empty<GUIContent>();
				m_BindingOptionValues = Array.Empty<string>();
				m_SelectedBindingOption = -1;
				return;
			}

			ReadOnlyArray<InputBinding> bindings = action.bindings;
			int bindingCount = bindings.Count;

			m_BindingOptions = new GUIContent[bindingCount];
			m_BindingOptionValues = new string[bindingCount];
			m_SelectedBindingOption = -1;

			string currentBindingId = this.bindings.stringValue;
			for (int i = 0; i < bindingCount; ++i)
			{
				InputBinding binding = bindings[i];
				string id = binding.id.ToString();
				bool haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

				// If we don't have a binding groups (control schemes), show the device that if there are, for example,
				// there are two bindings with the display string "A", the user can see that one is for the keyboard
				// and the other for the gamepad.
				InputBinding.DisplayStringOptions displayOptions =
					InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;

				if (!haveBindingGroups)
				{
					displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
				}

				// Create display string.
				string displayString = action.GetBindingDisplayString(i, displayOptions);

				// If binding is part of a composite, include the part name.
				if (binding.isPartOfComposite)
				{
					displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";
				}

				// Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
				// by instead using a backlash.
				displayString = displayString.Replace('/', '\\');

				// If the binding is part of control schemes, mention them.
				if (haveBindingGroups)
				{
					InputActionAsset asset = action.actionMap?.asset;
					if (asset != null)
					{
						string controlSchemes = string.Join(", ",
							binding.groups.Split(InputBinding.Separator)
							       .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

						displayString = $"{displayString} ({controlSchemes})";
					}
				}

				m_BindingOptions[i] = new GUIContent(displayString);
				m_BindingOptionValues[i] = id;

				if (currentBindingId == id)
				{
					m_SelectedBindingOption = i;
				}
			}
		}
	}
}
#endif