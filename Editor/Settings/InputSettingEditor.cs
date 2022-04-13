#if HERTZ_SETTINGS_INPUTSYSTEM
using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Hertzole.Settings.Editor
{
	[CustomEditor(typeof(InputSetting))]
	public class InputSettingEditor : SettingEditor
	{
		private readonly GUIContent bindingLabel = new GUIContent("Binding");
		private readonly GUIContent groupLabel = new GUIContent("Group");

		private GUIContent[] bindingOptions;

		private GUIContent[] groupOptions;

		private int[] selectedBindingOptionValues;
		private int[] selectedGroupOptionValues;

		private ReorderableList bindingsList;
		private SerializedProperty bindings;
		private SerializedProperty targetAction;

		private string[] bindingOptionValues;
		private string[] groupOptionValues;

		protected override void OnEnable()
		{
			base.OnEnable();

			targetAction = serializedObject.FindProperty(nameof(targetAction));
			bindings = serializedObject.FindProperty(nameof(bindings));

			bindingsList = new ReorderableList(serializedObject, bindings, true, true, true, true)
			{
				drawHeaderCallback = rect =>
				{
					Rect r = new Rect(rect.x + 16, rect.y, rect.width / 2 - 16, EditorGUIUtility.singleLineHeight);
					EditorGUI.LabelField(r, bindingLabel);
					r.x += rect.width / 2f - 8;
					EditorGUI.LabelField(r, groupLabel);
				},
				drawElementCallback = (rect, index, active, focused) =>
				{
					if (targetAction.objectReferenceValue == null)
					{
						return;
					}

					Rect r = new Rect(rect.x, rect.y + 3, rect.width / 2 - 2, EditorGUIUtility.singleLineHeight);
					int newSelectedBinding = EditorGUI.Popup(r, selectedBindingOptionValues[index], bindingOptions);
					if (newSelectedBinding != selectedBindingOptionValues[index])
					{
						UpdateItemBindingId(newSelectedBinding, index);
					}

					r.x += rect.width / 2f + 2;
					int newSelectedGroup = EditorGUI.Popup(r, selectedGroupOptionValues[index], groupOptions);
					if (newSelectedGroup != selectedGroupOptionValues[index])
					{
						UpdateItemGroup(newSelectedGroup, index);
					}
				},
				onAddCallback = list =>
				{
					int index = list.serializedProperty.arraySize;
					list.serializedProperty.arraySize++;
					list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("bindingId").stringValue = bindingOptionValues[0];
					list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("groups").stringValue = groupOptionValues[0];

					RefreshBindingOptions();
					RefreshGroupOptions();
				}
			};

			RefreshBindingOptions();
			RefreshGroupOptions();
		}

		private void UpdateItemGroup(int newSelectedGroup, int index)
		{
			string id = groupOptionValues[newSelectedGroup];
			bindings.GetArrayElementAtIndex(index).FindPropertyRelative("groups").stringValue = id;
			selectedGroupOptionValues[index] = newSelectedGroup;
		}

		private void UpdateItemBindingId(int newSelectedBinding, int index)
		{
			string id = bindingOptionValues[newSelectedBinding];
			bindings.GetArrayElementAtIndex(index).FindPropertyRelative("bindingId").stringValue = id;
			selectedBindingOptionValues[index] = newSelectedBinding;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(targetAction);

			EditorGUILayout.Space();

			if (targetAction.objectReferenceValue != null)
			{
				EditorGUILayout.HelpBox("Assign all the same bindings from different control schemes that the system should bind to.", MessageType.Info);

				bindingsList.DoLayoutList();
			}
			else
			{
				EditorGUILayout.HelpBox("Please assign a target action.", MessageType.Info);
			}

			if (EditorGUI.EndChangeCheck())
			{
				RefreshBindingOptions();
				RefreshGroupOptions();
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void RefreshBindingOptions()
		{
			if (targetAction.objectReferenceValue == null)
			{
				bindingOptions = Array.Empty<GUIContent>();
				bindingOptionValues = Array.Empty<string>();
				selectedBindingOptionValues = Array.Empty<int>();

				this.bindings.ClearArray();

				return;
			}

			InputActionReference actionReference = (InputActionReference) targetAction.objectReferenceValue;
			InputAction action = actionReference.action;

			if (action == null)
			{
				bindingOptions = Array.Empty<GUIContent>();
				bindingOptionValues = Array.Empty<string>();
				selectedBindingOptionValues = Array.Empty<int>();
				return;
			}

			ReadOnlyArray<InputBinding> bindings = action.bindings;
			int bindingCount = bindings.Count;

			bindingOptions = new GUIContent[bindingCount];
			bindingOptionValues = new string[bindingCount];
			selectedBindingOptionValues = new int[this.bindings.arraySize];

			// string currentBindingId = this.bindings.stringValue;
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

				bindingOptions[i] = new GUIContent(displayString);
				bindingOptionValues[i] = id;

				for (int j = 0; j < this.bindings.arraySize; ++j)
				{
					if (this.bindings.GetArrayElementAtIndex(j).FindPropertyRelative("bindingId").stringValue == id)
					{
						selectedBindingOptionValues[j] = i;
						break;
					}
				}
			}

			for (int i = 0; i < this.bindings.arraySize; i++)
			{
				bool foundBinding = false;
				
				var prop = this.bindings.GetArrayElementAtIndex(i).FindPropertyRelative("bindingId");
				for (int j = 0; j < bindingOptionValues.Length; j++)
				{
					if (prop.stringValue == bindingOptionValues[j])
					{
						foundBinding = true;
						selectedBindingOptionValues[i] = j;
						break;
					}
				}

				if (foundBinding)
				{
					continue;
				}

				UpdateItemBindingId(0, i);
			}
		}

		private void RefreshGroupOptions()
		{
			if (targetAction.objectReferenceValue == null)
			{
				groupOptions = Array.Empty<GUIContent>();
				groupOptionValues = Array.Empty<string>();
				selectedGroupOptionValues = Array.Empty<int>();
				return;
			}

			InputActionReference actionReference = (InputActionReference) targetAction.objectReferenceValue;

			int count = actionReference.asset.controlSchemes.Count;

			groupOptions = new GUIContent[count];
			groupOptionValues = new string[count];
			selectedGroupOptionValues = new int[bindings.arraySize];

			for (int i = 0; i < count; i++)
			{
				groupOptions[i] = new GUIContent(actionReference.asset.controlSchemes[i].name);
				groupOptionValues[i] = actionReference.asset.controlSchemes[i].bindingGroup;

				for (int j = 0; j < bindings.arraySize; ++j)
				{
					if (bindings.GetArrayElementAtIndex(j).FindPropertyRelative("groups").stringValue == actionReference.asset.controlSchemes[i].bindingGroup)
					{
						selectedGroupOptionValues[j] = i;
						break;
					}
				}
			}
		}
	}
}
#endif