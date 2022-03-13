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
	public class ResolutionSetting : Setting<Resolution>
	{
		protected Resolution[] uniqueResolutions = null;

		public override bool CanSave { get { return false; } }

		protected override Resolution TryConvertValue(object newValue)
		{
			return Screen.currentResolution;
		}

#if HERTZ_SETTINGS_UIELEMENTS
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

		protected virtual Resolution[] GetUniqueResolutions()
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
	}
}