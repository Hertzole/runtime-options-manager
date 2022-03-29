#if HERTZ_SETTINGS_INPUTSYSTEM
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hertzole.Settings
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

		public InputActionReference TargetAction { get { return targetAction; } }
		public BindingInfo[] Bindings { get { return bindings; } }

		public override object GetDefaultValue()
		{
			return default;
		}

		public override void SetSerializedValue(object newValue) { }

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
				if (bindings[i].groups == input.currentControlScheme)
				{
					return bindings[i].bindingId;
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
				if (bindings[i].groups == input.currentControlScheme)
				{
					Guid id = new Guid(bindings[i].bindingId);
					bindingIndex = action.bindings.IndexOf(x => x.id == id);
					if (bindingIndex == -1)
					{
						Debug.LogError($"Cannot find binding with ID '{bindings[i].bindingId}' in action '{action.name}'.", this);
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
			public string bindingId;
			public string groups;
		}
	}
}
#endif