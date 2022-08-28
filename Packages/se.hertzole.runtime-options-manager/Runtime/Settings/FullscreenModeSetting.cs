using System;
using System.Collections.Generic;
using UnityEngine;
#if HERTZ_SETTINGS_LOCALIZATION
using UnityEngine.Localization;
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

		private bool hasModes = false;
		private readonly List<ModeInfo> modes = new List<ModeInfo>(4);

		public override bool CanSave { get { return false; } }

		public ModeInfo ExclusiveFullscreen { get { return exclusiveFullscreen; } set { exclusiveFullscreen = value; } }
		public ModeInfo BorderlessFullscreen { get { return borderlessFullscreen; } set { borderlessFullscreen = value; } }
		public ModeInfo MaximizedWindow { get { return maximizedWindow; } set { maximizedWindow = value; } }
		public ModeInfo Windowed { get { return windowed; } set { windowed = value; } }

		protected override void SetValue(FullScreenMode newValue)
		{
			if (value != newValue)
			{
				InvokeOnValueChanging(value);
				value = newValue;
				InvokeOnValueChanged(value);
				InvokeOnSettingChanged();

				Screen.fullScreenMode = value;
			}
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			// Does nothing, Unity handles this.
			value = Screen.fullScreenMode;
		}

		protected override FullScreenMode TryConvertValue(object newValue)
		{
			if (newValue is int intMode)
			{
				switch (intMode)
				{
					case 0:
						return FullScreenMode.ExclusiveFullScreen;
					case 1:
						return FullScreenMode.FullScreenWindow;
					case 2:
						return FullScreenMode.MaximizedWindow;
					case 3:
						return FullScreenMode.Windowed;
					default:
						Debug.LogError($"Fullscreen mode with int value {intMode} is not supported.");
						return DefaultValue;
				}
			}

			return DefaultValue;
		}

		private int IndexOf(FullScreenMode mode)
		{
			GetModes();

			for (int i = 0; i < modes.Count; i++)
			{
				if (modes[i].fullScreenMode == mode)
				{
					return i;
				}
			}

			return -1;
		}

		private void GetModes()
		{
			if (hasModes)
			{
				return;
			}

			if (exclusiveFullscreen.isEnabled)
			{
				exclusiveFullscreen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				modes.Add(exclusiveFullscreen);
			}

			if (borderlessFullscreen.isEnabled)
			{
				borderlessFullscreen.fullScreenMode = FullScreenMode.FullScreenWindow;
				modes.Add(borderlessFullscreen);
			}

			if (maximizedWindow.isEnabled)
			{
				maximizedWindow.fullScreenMode = FullScreenMode.MaximizedWindow;
				modes.Add(maximizedWindow);
			}

			if (windowed.isEnabled)
			{
				windowed.fullScreenMode = FullScreenMode.Windowed;
				modes.Add(windowed);
			}

			hasModes = true;
		}

		public ModeInfo GetModeAtIndex(int index)
		{
			GetModes();

			return modes[index];
		}
		
		public void SetDropdownValue(int index)
		{
			GetModes();

			SetValue(modes[index].fullScreenMode);
		}

		public int GetDropdownValue()
		{
			GetModes();

			int index = IndexOf(Value);

			return index == -1 ? 0 : index;
		}

		public IReadOnlyList<(string text, Sprite icon)> GetDropdownValues()
		{
			GetModes();

			(string, Sprite)[] result = new (string, Sprite)[modes.Count];
			for (int i = 0; i < modes.Count; i++)
			{
				result[i] = (modes[i].name, modes[i].icon);
			}

			return result;
		}

		public override void ResetState()
		{
			base.ResetState();

			hasModes = false;
			modes.Clear();
		}

		[Serializable]
		public class ModeInfo
		{
			public bool isEnabled;

			public string name;
#if HERTZ_SETTINGS_LOCALIZATION
			public LocalizedString localizedName;
#endif

			public FullScreenMode fullScreenMode;
			public Sprite icon;

			public ModeInfo(bool isEnabled, string name)
			{
				this.isEnabled = isEnabled;
				this.name = name;
			}
		}
	}
}