using UnityEngine;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New VSync Setting", menuName = "Hertzole/Settings/VSync Settings")]
#endif
	public class VSyncSetting : ToggleSetting
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

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			base.SetSerializedValue(newValue, serializer);
			QualitySettings.vSyncCount = Value ? 1 : 0;
		}

		// #if HERTZ_SETTINGS_UIELEMENTS
// 		public override VisualElement CreateUIElement()
// 		{
// 			if (uiElement == null)
// 			{
// 				return null;
// 			}
//
// 			TemplateContainer ui = uiElement.CloneTree();
// 			Toggle toggle = ui.Q<Toggle>();
// 			toggle.label = DisplayName;
// 			toggle.SetValueWithoutNotify(Value);
//
// 			toggle.RegisterValueChangedCallback(evt => { Value = evt.newValue; });
//
// 			return ui;
// 		}
// #endif
	}
}