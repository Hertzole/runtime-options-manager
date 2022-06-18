using UnityEngine;
using UnityEngine.Events;

namespace Hertzole.OptionsManager
{
	[DefaultExecutionOrder(-100)] // Must run AFTER SettingsManager.
	public abstract class SettingListener<TValue, TSetting> : MonoBehaviour where TSetting : Setting<TValue>
	{
		[SerializeField]
		private TSetting setting = default;

		[SerializeField]
		private UnityEvent<TValue> onValueChanging = new UnityEvent<TValue>();
		[SerializeField]
		private UnityEvent<TValue> onValueChanged = new UnityEvent<TValue>();

		public TSetting Setting
		{
			get
			{
				return setting;
			}
			set
			{
				if (setting != null)
				{
					setting.OnValueChanging -= InvokeOnValueChanging;
					setting.OnValueChanged -= InvokeOnValueChanged;
				}
				setting = value;
				if (setting != null)
				{
					setting.OnValueChanging += InvokeOnValueChanging;
					setting.OnValueChanged += InvokeOnValueChanged;
				}
			}
		}

		public UnityEvent<TValue> OnValueChanging { get { return onValueChanging; } set { onValueChanging = value; } }
		public UnityEvent<TValue> OnValueChanged { get { return onValueChanged; } set { onValueChanged = value; } }

		private void Start()
		{
			if (setting != null)
			{
				InvokeOnValueChanged(setting.Value);
			}
		}

		private void OnEnable()
		{
			if (setting != null)
			{
				setting.OnValueChanging += InvokeOnValueChanging;
				setting.OnValueChanged += InvokeOnValueChanged;
			}
		}

		private void OnDisable()
		{
			if (setting != null)
			{
				setting.OnValueChanging -= InvokeOnValueChanging;
				setting.OnValueChanged -= InvokeOnValueChanged;
			}
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