using System;
using UnityEngine;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif

namespace Hertzole.RuntimeOptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New VSync Setting", menuName = "Hertzole/Settings/VSync Settings")]
#endif
	public class VSyncSetting : Setting<bool>
	{
		protected override void SetValue(bool newValue)
		{
			if (!value.Equals(newValue))
			{
				InvokeOnValueChanging(newValue);
				value = newValue;
				QualitySettings.vSyncCount = newValue ? 1 : 0;
				InvokeOnValueChanged(newValue);
				InvokeOnSettingChanged();
			}
		}

		protected override bool TryConvertValue(object newValue)
		{
			return Convert.ToBoolean(newValue);
		}

#if HERTZ_SETTINGS_UIELEMENTS
		public override VisualElement CreateUIElement()
		{
			if (uiElement == null)
			{
				return null;
			}

			TemplateContainer ui = uiElement.CloneTree();
			Toggle toggle = ui.Q<Toggle>();
			toggle.label = DisplayName;
			toggle.SetValueWithoutNotify(Value);

			toggle.RegisterValueChangedCallback(evt => { Value = evt.newValue; });

			return ui;
		}
#endif
	}
}