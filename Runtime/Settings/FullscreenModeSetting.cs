using System;
using System.Collections.Generic;
using UnityEngine;
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization;
#endif

#if HERTZ_SETTINGS_UIELEMENTS && UNITY_2021_2_OR_NEWER
using UnityEngine.UIElements;
#endif

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Fullscreen Mode Setting", menuName = "Hertzole/Settings/Fullscreen Mode Setting")]
#endif
	public partial class FullscreenModeSetting : Setting<FullScreenMode>, IDropdownValues
	{
		[SerializeField]
		private ModeInfo exclusiveFullscreen = new ModeInfo(true, "Exclusive Fullscreen");
		[SerializeField]
		private ModeInfo borderlessFullscreen = new ModeInfo(true, "Borderless Fullscreen");
		[SerializeField]
		private ModeInfo maximizedWindow = new ModeInfo(false, "Maximized Window");
		[SerializeField]
		private ModeInfo windowed = new ModeInfo(true, "Windowed");

		private readonly List<FullScreenMode> modes = new List<FullScreenMode>(4);

		public override bool CanSave { get { return false; } }

		protected override void SetValue(FullScreenMode newValue)
		{
			if (!value.Equals(newValue))
			{
				InvokeOnValueChanging(value);
				value = newValue;
				InvokeOnValueChanged(value);
				InvokeOnSettingChanged();

				Screen.fullScreenMode = value;
			}
		}

		protected override FullScreenMode TryConvertValue(object newValue)
		{
			return Screen.fullScreenMode;
		}

// #if HERTZ_SETTINGS_UIELEMENTS && UNITY_2021_2_OR_NEWER
// 		public override VisualElement CreateUIElement()
// 		{
// 			if (uiElement == null)
// 			{
// 				return null;
// 			}
//
// 			var ui = uiElement.CloneTree();
// 			var dropdown = ui.Q<DropdownField>();
// 			dropdown.choices.Clear();
// 			dropdown.label = DisplayName;
//
// 			dropdown.choices.Add("Windowed");
// 			dropdown.choices.Add("Borderless Fullscreen");
// 			dropdown.choices.Add("Exclusive Fullscreen");
//
// 			switch (Screen.fullScreenMode)
// 			{
// 				case FullScreenMode.ExclusiveFullScreen:
// 					dropdown.index = 2;
// 					break;
// 				case FullScreenMode.FullScreenWindow:
// 					dropdown.index = 1;
// 					break;
// 				case FullScreenMode.MaximizedWindow:
// 				case FullScreenMode.Windowed:
// 					dropdown.index = 0;
// 					break;
// 				default:
// 					throw new ArgumentOutOfRangeException();
// 			}
//
// 			dropdown.RegisterValueChangedCallback(evt =>
// 			{
// 				switch (dropdown.index)
// 				{
// 					case 0:
// 						Screen.fullScreenMode = FullScreenMode.Windowed;
// 						break;
// 					case 1:
// 						Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
// 						break;
// 					case 2:
// 						Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
// 						break;
// 				}
// 			});
// 			
// 			return ui;
// 		}
// #endif
		public void SetDropdownValue(int index)
		{
			SetValue(modes[index]);
		}

		public int GetDropdownValue()
		{
			return modes.IndexOf(Screen.fullScreenMode);
		}

		public IReadOnlyList<(string text, Sprite icon)> GetDropdownValues()
		{
			bool firstTimeSetup = modes.Count == 0;

			List<(string, Sprite)> list = new List<(string, Sprite)>(4);
			if (exclusiveFullscreen.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.ExclusiveFullScreen);
				}

				list.Add((exclusiveFullscreen.name, null));
			}

			if (borderlessFullscreen.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.FullScreenWindow);
				}

				list.Add((borderlessFullscreen.name, null));
			}

			if (maximizedWindow.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.MaximizedWindow);
				}

				list.Add((maximizedWindow.name, null));
			}

			if (windowed.isEnabled)
			{
				if (firstTimeSetup)
				{
					modes.Add(FullScreenMode.Windowed);
				}

				list.Add((windowed.name, null));
			}

			return list;
		}

		[Serializable]
		public struct ModeInfo
		{
			public bool isEnabled;

			public string name;
#if HERTZ_SETTINGS_LOCALIZATION
			public LocalizedString localizedName;
#endif

			public ModeInfo(bool isEnabled, string name) : this()
			{
				this.isEnabled = isEnabled;
				this.name = name;
			}
		}
	}
}