﻿using UnityEngine;
using UnityEngine.Events;

namespace Hertzole.Settings
{
	[DefaultExecutionOrder(-100)] // Must run AFTER SettingsManager.
	public abstract class SettingListener<TValue, TSetting> : MonoBehaviour where TSetting : Setting<TValue>
	{
		[SerializeField]
		private TSetting setting = default;

		[SerializeField]
		private UnityEvent<TValue> onValueChanging = default;
		[SerializeField]
		private UnityEvent<TValue> onValueChanged = default;

		public UnityEvent<TValue> OnValueChanging { get { return onValueChanging; } set { onValueChanging = value; } }
		public UnityEvent<TValue> OnValueChanged { get { return onValueChanged; } set { onValueChanged = value; } }

		private void Start()
		{
			InvokeOnValueChanged(setting.Value);
		}

		private void OnEnable()
		{
			setting.OnValueChanging += InvokeOnValueChanging;
			setting.OnValueChanged += InvokeOnValueChanged;
		}

		private void OnDisable()
		{
			setting.OnValueChanging -= InvokeOnValueChanging;
			setting.OnValueChanged -= InvokeOnValueChanged;
		}

		private void InvokeOnValueChanging(TValue value)
		{
			onValueChanging.Invoke(value);
		}

		private void InvokeOnValueChanged(TValue value)
		{
			onValueChanged.Invoke(value);
		}
	}
}