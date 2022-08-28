using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.OptionsManager
{
	public abstract class Setting<T> : Setting
	{
		[SerializeField]
		private T defaultValue = default;

		protected T value;

		public T DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
		public T Value { get { return GetValue(); } set { SetValue(value); } }

		public event Action<T> OnValueChanging;
		public event Action<T> OnValueChanged;

		public override void ResetState()
		{
			value = defaultValue;
		}
		
		protected virtual T GetValue()
		{
			return value;
		}

		protected virtual void SetValue(T newValue)
		{
			if (!EqualityComparer<T>.Default.Equals(Value, newValue))
			{
				OnValueChanging?.Invoke(value);
				value = newValue;
				OnValueChanged?.Invoke(value);
				InvokeOnSettingChanged();
			}
		}

		public override object GetDefaultValue()
		{
			return defaultValue;
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			// We don't want to invoke the OnSettingChanged event when setting the serialized value as that will cause
			// the settings manager to save the settings at boot.
			DontInvokeSettingChanged = true;
			if (newValue is T convertedValue)
			{
				Value = convertedValue;
			}
			else
			{
				try
				{
					Value = serializer.DeserializeType<T>(newValue);
				}
				catch (Exception)
				{
					Value = TryConvertValue(newValue);
				}
			}

			DontInvokeSettingChanged = false;
		}

		public override object GetSerializeValue()
		{
			return GetValue();
		}

		protected abstract T TryConvertValue(object newValue);

		protected void InvokeOnValueChanging(T value)
		{
			OnValueChanging?.Invoke(value);
		}

		protected void InvokeOnValueChanged(T value)
		{
			OnValueChanged?.Invoke(value);
		}
	}
}