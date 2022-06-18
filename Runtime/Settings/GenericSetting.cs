using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.OptionsManager
{
	public abstract class Setting<T> : Setting
	{
#if UNITY_EDITOR
		[Header("Value Settings")]
#endif
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
			if (!EqualityComparer<T>.Default.Equals(value, newValue))
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
			if (newValue is T convertedValue)
			{
				value = convertedValue;
			}
			else
			{
				try
				{
					value = serializer.DeserializeType<T>(newValue);
				}
				catch (ArgumentException)
				{
					value = TryConvertValue(newValue);
				}
			}
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