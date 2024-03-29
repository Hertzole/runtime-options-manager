﻿#if HERTZ_SETTINGS_INPUTSYSTEM
using System;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Input Setting", menuName = "Hertzole/Settings/Input Setting")]
#endif
	public class InputSetting : Setting
	{
		[SerializeField]
		private InputActionReference targetAction = default;
		[SerializeField]
		private BindingInfo[] bindings = default;

		public InputActionReference TargetAction { get { return targetAction; } set {targetAction = value; } }
		public BindingInfo[] Bindings { get { return bindings; } set { bindings = value; } }

		public override object GetDefaultValue()
		{
			return default;
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			// Check if it's an InputActionData. If it isn't try to make it one.
			if (newValue is InputActionData actionData)
			{
				if (targetAction == null || targetAction.action == null || targetAction.action.id.ToString() != actionData.id)
				{
					return;
				}

				ReadOnlyArray<InputBinding> actionBindings = targetAction.action.bindings;
				for (int i = 0; i < actionData.bindings.Count; i++)
				{
					if (!actionData.bindings[i].isOverwritten)
					{
						continue;
					}

					for (int j = 0; j < actionBindings.Count; j++)
					{
						if (actionBindings[j].id.ToString() == actionData.bindings[i].id)
						{
							InputBinding binding = actionBindings[j];
							binding.overridePath = actionData.bindings[i].path;
							targetAction.action.ApplyBindingOverride(j, binding);
							break;
						}
					}
				}
			}
			else
			{
				InputActionData newData = serializer.DeserializeType<InputActionData>(newValue);
				SetSerializedValue(newData, serializer);
			}
		}

		public override object GetSerializeValue()
		{
			if(targetAction == null || targetAction.action == null)
			{
				return null;
			}
			
			string actionId = targetAction.action.id.ToString();

			InputBindingData[] bindingData = new InputBindingData[bindings.Length];

			for (int i = 0; i < bindings.Length; i++)
			{
				string bindingId = bindings[i].BindingId;
				string path = string.Empty;
				ReadOnlyArray<InputBinding> actionBindings = targetAction.action.bindings;
				bool hasOverrides = false;
				for (int j = 0; j < actionBindings.Count; j++)
				{
					if (actionBindings[j].id.ToString() == bindingId)
					{
						hasOverrides = actionBindings[j].hasOverrides;
						path = hasOverrides ? actionBindings[j].overridePath : actionBindings[j].path;
						break;
					}
				}

				bindingData[i] = new InputBindingData(bindingId, path, hasOverrides);
			}

			return new InputActionData(actionId, bindingData);
		}

		public InputActionRebindingExtensions.RebindingOperation StartRebind(PlayerInput targetInput, out bool isComposite, out int bindingIndex)
		{
			isComposite = false;
			InputAction action = targetInput.actions.FindAction(targetAction.action.id);
			if (!ResolveActionAndBinding(targetInput, action, out bindingIndex))
			{
				return null;
			}

			return StartRebind(action, bindingIndex, out isComposite);
		}

		private InputActionRebindingExtensions.RebindingOperation StartRebind(InputAction action, int bindingIndex, out bool isComposite)
		{
			isComposite = action.bindings[bindingIndex].isComposite;
			action.Disable();
			return action.PerformInteractiveRebinding(bindingIndex);
		}

		public void FinishRebind()
		{
			InvokeOnSettingChanged();
		}

		public bool ResetToDefault(PlayerInput targetInput)
		{
			InputAction action = targetInput.actions.FindAction(targetAction.action.id);
			if (!ResolveActionAndBinding(targetInput, action, out int bindingIndex))
			{
				return false;
			}

			if (action.bindings[bindingIndex].isComposite)
			{
				for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
				{
					action.RemoveBindingOverride(i);
				}
			}
			else
			{
				action.RemoveBindingOverride(bindingIndex);
			}

			return true;
		}

		public string GetCurrentBindingId(PlayerInput input)
		{
			for (int i = 0; i < bindings.Length; i++)
			{
				if (bindings[i].Groups == input.currentControlScheme)
				{
					return bindings[i].BindingId;
				}
			}

			Debug.LogError($"Cannot find a target binding for control scheme '{input.currentControlScheme}' in '{name}'.", this);

			return string.Empty;
		}

		private bool ResolveActionAndBinding(PlayerInput input, InputAction action, out int bindingIndex)
		{
			bindingIndex = -1;

			for (int i = 0; i < bindings.Length; i++)
			{
				if (bindings[i].Groups == input.currentControlScheme)
				{
					Guid id = new Guid(bindings[i].BindingId);
					bindingIndex = action.bindings.IndexOf(x => x.id == id);
					if (bindingIndex == -1)
					{
						Debug.LogError($"Cannot find binding with ID '{bindings[i].BindingId}' in action '{action.name}'.", this);
						return false;
					}

					return true;
				}
			}

			Debug.LogError($"Cannot find a target binding for control scheme '{input.currentControlScheme}' in '{action.name}'.", this);
			return false;
		}

		[Serializable]
		public struct BindingInfo
		{
			[SerializeField]
			private string bindingId;
			[SerializeField]
			private string groups;

			public string BindingId { get { return bindingId; } set { bindingId = value; } }
			public string Groups { get { return groups; } set { groups = value; } }

			public BindingInfo(string bindingId, string groups)
			{
				this.bindingId = bindingId;
				this.groups = groups;
			}
		}
	}
}
#endif