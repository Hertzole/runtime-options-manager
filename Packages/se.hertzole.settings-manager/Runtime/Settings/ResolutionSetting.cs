using System.Collections.Generic;
using UnityEngine;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif

namespace Hertzole.Settings
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Resolution Setting", menuName = "Hertzole/Settings/Resolution Setting")]
#endif
	public class ResolutionSetting : Setting<Resolution>, IDropdownValues
	{
		[SerializeField]
		private string resolutionFormat = "{0}x{1}";
		
		protected Resolution[] uniqueResolutions = null;

		public override bool CanSave { get { return false; } }

		protected override void SetValue(Resolution newValue)
		{
			if (!value.Equals(newValue))
			{
				InvokeOnValueChanging(value);
				value = newValue;
				InvokeOnValueChanged(value);
				InvokeOnSettingChanged();

				Screen.SetResolution(newValue.width, newValue.height, Screen.fullScreenMode);
			}
		}

		protected override Resolution TryConvertValue(object newValue)
		{
			return Screen.currentResolution;
		}

#if HERTZ_SETTINGS_UIELEMENTS && UNITY_2021_2_OR_NEWER
		public override VisualElement CreateUIElement()
		{
			if (uiElement == null)
			{
				return null;
			}

			TemplateContainer ui = uiElement.CloneTree();
			DropdownField dropdown = ui.Q<DropdownField>();
			Resolution[] resolutions = GetUniqueResolutions();

			dropdown.choices.Clear();
			dropdown.label = DisplayName;

			int selectedOption = 0;

			var currentWidth = Screen.width;
			var currentHeight = Screen.height;
			
			for (int i = 0; i < resolutions.Length; i++)
			{
				if (resolutions[i].width == currentWidth && resolutions[i].height == currentHeight)
				{
					selectedOption = i;
				}
				
				dropdown.choices.Add($"{resolutions[i].width}x{resolutions[i].height}");
			}

			dropdown.SetValueWithoutNotify(dropdown.choices[selectedOption]);
			dropdown.RegisterValueChangedCallback(evt =>
			{
				Screen.SetResolution(resolutions[dropdown.index].width, resolutions[dropdown.index].height, Screen.fullScreenMode);
			});

			return ui;
		}
#endif

		public virtual Resolution[] GetUniqueResolutions()
		{
			if (uniqueResolutions != null)
			{
				return uniqueResolutions;
			}

			Resolution[] allResolutions = Screen.resolutions;
			List<Resolution> resolutions = new List<Resolution>();

			int refreshRate = Screen.currentResolution.refreshRate;

			for (int i = 0; i < allResolutions.Length; i++)
			{
				Resolution data = new Resolution
				{
					width = allResolutions[i].width,
					height = allResolutions[i].height,
					refreshRate = refreshRate
				};

				if (!resolutions.Contains(data))
				{
					resolutions.Add(allResolutions[i]);
				}
			}

			uniqueResolutions = resolutions.ToArray();

			return uniqueResolutions;
		}

		public void SetDropdownValue(int index)
		{
			GetUniqueResolutions();
			
			Screen.SetResolution(uniqueResolutions[index].width, uniqueResolutions[index].height, Screen.fullScreen);
		}

		public int GetDropdownValue()
		{
			GetUniqueResolutions();
			
			int index = 0;

			for (int i = 0; i < uniqueResolutions.Length; i++)
			{
				if (uniqueResolutions[i].width == Screen.width && uniqueResolutions[i].height == Screen.height)
				{
					index = i;
					break;
				}
			}

			return index;
		}

		public IReadOnlyList<(string text, Sprite icon)> GetDropdownValues()
		{
			GetUniqueResolutions();
			
			List<(string text, Sprite icon)> values = new List<(string text, Sprite icon)>();

			for (int i = 0; i < uniqueResolutions.Length; i++)
			{
				values.Add((string.Format(resolutionFormat, uniqueResolutions[i].width, uniqueResolutions[i].height), null));
			}

			return values;
		}
	}
}