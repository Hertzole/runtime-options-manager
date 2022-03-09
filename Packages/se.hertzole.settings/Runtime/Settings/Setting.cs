using System;
using UnityEngine;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Hertzole.Settings
{
	public abstract class Setting : ScriptableObject
	{
#if UNITY_EDITOR
		[Header("General Settings")]
#endif
		[SerializeField]
		private string displayName = "New Setting";
#if HERTZ_SETTINGS_LOCALIZATION
		[SerializeField]
		private LocalizedString displayNameLocalized = default;
#endif
		[SerializeField]
		private string identifier = "new_setting";

#if UNITY_EDITOR
		[Header("UI")]
#endif
#if HERTZ_SETTINGS_UIELEMENTS
		[SerializeField]
		protected VisualTreeAsset uiElement = default;
#endif

		public string DisplayName { get { return displayName; } set { displayName = value; } }
#if HERTZ_SETTINGS_LOCALIZATION
		public LocalizedString DisplayNameLocalized { get { return displayNameLocalized; } set { displayNameLocalized = value; } }
#endif
		public string Identifier { get { return identifier; } set { identifier = value; } }
		
		public virtual bool CanSave { get { return true; } }

		public event Action OnSettingChanged;

		public abstract void SetSerializedValue(object newValue);
		
		public virtual object GetSerializeValue()
		{
			return null;
		}
		
		protected void InvokeOnSettingChanged()
		{
			OnSettingChanged?.Invoke();
		}

#if HERTZ_SETTINGS_UIELEMENTS
		public virtual VisualElement CreateUIElement()
		{
			return uiElement == null ? null : uiElement.CloneTree();
		}
#endif
	}

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

		protected virtual T GetValue()
		{
			return value;
		}

		protected virtual void SetValue(T newValue)
		{
			if (!value.Equals(newValue))
			{
				OnValueChanging?.Invoke(value);
				value = newValue;
				OnValueChanged?.Invoke(value);
				InvokeOnSettingChanged();
			}
		}

		public override void SetSerializedValue(object newValue)
		{
			if (newValue is T convertedValue)
			{
				value = convertedValue;
			}
			else
			{
				value = TryConvertValue(newValue);
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