using System;
using Hertzole.Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputSettingElement : MonoBehaviour
{
	[SerializeField]
	private Text label = default;
	[SerializeField]
	private Button button = default;
	[SerializeField]
	private Text buttonLabel = default;
	[SerializeField]
	private InputBinding.DisplayStringOptions displayStringOptions = default;

	private PlayerInput playerInput;
	private InputSetting inputSetting;

	private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

	private void Awake()
	{
		button.onClick.AddListener(OnClick);
	}

	private void OnDestroy()
	{
		if (playerInput != null)
		{
			playerInput.onControlsChanged -= OnControlsChanged;
		}
	}

	private void OnClick()
	{
		PerformRebind();
	}

	public void BindSetting(PlayerInput input, InputSetting setting)
	{
		playerInput = input;
		label.text = setting.DisplayName;
		inputSetting = setting;
		
		UpdateBindingDisplay();
		playerInput.onControlsChanged += OnControlsChanged;
	}
	
	private void OnControlsChanged(PlayerInput obj)
	{
		UpdateBindingDisplay();
	}

	private void PerformRebind()
	{
		rebindingOperation?.Cancel();

		rebindingOperation = inputSetting.StartRebind(playerInput, out bool isComposite, out int bindingIndex)
		                                 .OnCancel(o =>
		                                 {
			                                 UpdateBindingDisplay();
			                                 CleanUp();
		                                 })
		                                 .OnComplete(o =>
		                                 {
			                                 UpdateBindingDisplay();
			                                 CleanUp();
		                                 });

		buttonLabel.text = "Waiting...";

		rebindingOperation.Start();
	}

	private void CleanUp()
	{
		rebindingOperation?.action.Enable();
		rebindingOperation?.Dispose();
		rebindingOperation = null;
	}

	private void UpdateBindingDisplay()
	{
		string displayString = string.Empty;

		// Get display string from action.
		InputAction action = inputSetting.TargetAction.action;
		if (action != null)
		{
			int bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == inputSetting.GetCurrentBindingId(playerInput));
			if (bindingIndex != -1)
			{
				displayString = action.GetBindingDisplayString(bindingIndex, out _, out _, displayStringOptions);
			}
		}

		// Set on label (if any).
		buttonLabel.text = displayString;
	}
}