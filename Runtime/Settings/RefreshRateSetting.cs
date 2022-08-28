using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Refresh Rate Setting", menuName = "Hertzole/Settings/Refresh Rate Setting")]
#endif
	public class RefreshRateSetting : Setting<int>, IDropdownValues
	{
		protected int[] uniqueRefreshRates = null;

		protected override void SetValue(int newValue)
		{
			if (newValue != Value)
			{
				InvokeOnValueChanging(value);
				value = newValue;
				InvokeOnValueChanged(value);
				InvokeOnSettingChanged();
				Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode, value);
			}

			Application.targetFrameRate = value;
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			switch (newValue)
			{
				case null:
					Value = -1;
					return;
				case string stringValue: // Catch string values because the JSON serialize will try to parse invalid strings and fail.
					Value = TryConvertValue(stringValue);
					return;
				default:
					base.SetSerializedValue(newValue, serializer);
					break;
			}
		}

		protected override int TryConvertValue(object newValue)
		{
			switch (newValue)
			{
				case string stringValue:
					return int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out int result) ? result : Screen.currentResolution.refreshRate;
				default:
					return Screen.currentResolution.refreshRate;
			}
		}

		public virtual int[] GetUniqueRefreshRates()
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

		public void SetDropdownValue(int index)
		{
			int[] refreshRates = GetUniqueRefreshRates();

			Value = refreshRates[index];
		}

		public int GetDropdownValue()
		{
			int[] refreshRates = GetUniqueRefreshRates();

			int currentRefreshRate = Value;

			for (int i = 0; i < refreshRates.Length; i++)
			{
				if (refreshRates[i] == currentRefreshRate)
				{
					return i;
				}
			}

			return 0;
		}

		public IReadOnlyList<(string text, Sprite icon)> GetDropdownValues()
		{
			int[] refreshRates = GetUniqueRefreshRates();

			List<(string text, Sprite icon)> result = new List<(string text, Sprite icon)>(refreshRates.Length);

			for (int i = 0; i < refreshRates.Length; i++)
			{
				result.Add((refreshRates[i].ToString(), null));
			}

			result.TrimExcess();
			return result;
		}
	}
}