using System;
using UnityEngine;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif

namespace Hertzole.Settings
{
	#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Fullscreen Mode Setting", menuName = "Hertzole/Settings/Fullscreen Mode Setting")]
	#endif
	public class FullscreenModeSetting : Setting<FullScreenMode>
	{
		public override bool CanSave { get { return false; } }

		protected override FullScreenMode TryConvertValue(object newValue)
		{
			return Screen.fullScreenMode;
		}
		
#if HERTZ_SETTINGS_UIELEMENTS && UNITY_2021_2_OR_NEWER
		public override VisualElement CreateUIElement()
		{
			if (uiElement == null)
			{
				return null;
			}

			var ui = uiElement.CloneTree();
			var dropdown = ui.Q<DropdownField>();
			dropdown.choices.Clear();
			dropdown.label = DisplayName;

			dropdown.choices.Add("Windowed");
			dropdown.choices.Add("Borderless Fullscreen");
			dropdown.choices.Add("Exclusive Fullscreen");

			switch (Screen.fullScreenMode)
			{
				case FullScreenMode.ExclusiveFullScreen:
					dropdown.index = 2;
					break;
				case FullScreenMode.FullScreenWindow:
					dropdown.index = 1;
					break;
				case FullScreenMode.MaximizedWindow:
				case FullScreenMode.Windowed:
					dropdown.index = 0;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			dropdown.RegisterValueChangedCallback(evt =>
			{
				switch (dropdown.index)
				{
					case 0:
						Screen.fullScreenMode = FullScreenMode.Windowed;
						break;
					case 1:
						Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
						break;
					case 2:
						Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
						break;
				}
			});
			
			return ui;
		}
#endif
	}
}