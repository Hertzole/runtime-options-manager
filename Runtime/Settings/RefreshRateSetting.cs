using System.Collections.Generic;
using UnityEngine;
#if HERTZ_SETTINGS_UIELEMENTS
using UnityEngine.UIElements;
#endif

namespace Hertzole.Settings
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Refresh Rate Setting", menuName = "Hertzole/Settings/Refresh Rate Setting")]
#endif
	public class RefreshRateSetting : Setting<int>
	{
		protected int[] uniqueRefreshRates = null;

		public override bool CanSave { get { return false; } }

		protected override int TryConvertValue(object newValue)
		{
			return Screen.currentResolution.refreshRate;
		}

#if HERTZ_SETTINGS_UIELEMENTS
		public override VisualElement CreateUIElement()
		{
			if (uiElement == null)
			{
				return null;
			}
		
			int[] refreshRates = GetUniqueRefreshRates();
			if (refreshRates.Length < 2)
			{
				return null;
			}

			var ui = uiElement.CloneTree();
			DropdownField dropdown = ui.Q<DropdownField>();
			dropdown.label = DisplayName;
			dropdown.choices.Clear();

			int selectedRefreshRate = 0;
			int currentRefreshRate = Screen.currentResolution.refreshRate;

			for (int i = 0; i < refreshRates.Length; i++)
			{
				if (refreshRates[i] == currentRefreshRate)
				{
					selectedRefreshRate = i;
				}

				dropdown.choices.Add(refreshRates[i].ToString());
			}

			Debug.Log(dropdown.choices.Count);

			dropdown.SetValueWithoutNotify(dropdown.choices[selectedRefreshRate]);
			dropdown.RegisterValueChangedCallback(evt =>
			{
				Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode, refreshRates[dropdown.index]);
			});

			return ui;
		}
#endif

		protected virtual int[] GetUniqueRefreshRates()
		{
			if (uniqueRefreshRates != null)
			{
				return uniqueRefreshRates;
			}

			Resolution[] allResolutions = Screen.resolutions;
			List<int> refreshRates = new List<int>();

			for (int i = 0; i < allResolutions.Length; i++)
			{
				if (!refreshRates.Contains(allResolutions[i].refreshRate))
				{
					refreshRates.Add(allResolutions[i].refreshRate);
				}
			}

			refreshRates.Sort();

			uniqueRefreshRates = refreshRates.ToArray();

			return uniqueRefreshRates;
		}
	}
}