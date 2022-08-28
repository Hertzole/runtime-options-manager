using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Resolution Setting", menuName = "Hertzole/Settings/Resolution Setting")]
#endif
	public class ResolutionSetting : Setting<Resolution>, IDropdownValues
	{
		[SerializeField]
		private string resolutionFormat = "{0}x{1}";

		protected Resolution[] uniqueResolutions = null;

		private static readonly ResolutionComparer resolutionComparer = new ResolutionComparer();

		public string ResolutionFormat { get { return resolutionFormat; } set { resolutionFormat = value; } }

		public override bool CanSave { get { return false; } }

		protected override void SetValue(Resolution newValue)
		{
			if (!EqualityComparer<Resolution>.Default.Equals(Value, newValue))
			{
				InvokeOnValueChanging(value);
				value = newValue;
				InvokeOnValueChanged(value);
				InvokeOnSettingChanged();

				Screen.SetResolution(newValue.width, newValue.height, Screen.fullScreenMode);
			}
		}

		public override void SetSerializedValue(object newValue, ISettingSerializer serializer)
		{
			// Unity handles resolutions itself so we don't need to do anything. Just set the value to the current resolution.
			value = TryConvertValue(newValue);
		}

		protected override Resolution TryConvertValue(object newValue)
		{
			// This method is not used like the others. Since Unity handles the resolution itself we just return the current resolution.
			
			// Can't use Screen.currentResolution here because that returns the whole monitor resolution and not the
			// game resolution.
			return new Resolution
			{
				width = Screen.width,
				height = Screen.height,
				refreshRate = Screen.currentResolution.refreshRate
			};
		}

		public virtual Resolution[] GetUniqueResolutions()
		{
			if (uniqueResolutions != null)
			{
				return uniqueResolutions;
			}

			Resolution[] allResolutions = Screen.resolutions;

			int refreshRate = Screen.currentResolution.refreshRate;

			int count = 0;
			for (int i = 0; i < allResolutions.Length; i++)
			{
				if (allResolutions[i].refreshRate == refreshRate)
				{
					count++;
				}
			}

			uniqueResolutions = new Resolution[count];

			int index = 0;

			for (int i = 0; i < allResolutions.Length; i++)
			{
				if (allResolutions[i].refreshRate == refreshRate)
				{
					uniqueResolutions[index] = allResolutions[i];
					index++;
				}
			}

			// Resolution[] tempResolutions = new Resolution[]
			// {
			// 	new Resolution { width = 300, height = 300, refreshRate = 60 },
			// 	new Resolution { width = 300, height = 300, refreshRate = 120 },
			// 	new Resolution { width = 300, height = 300, refreshRate = 165 },
			// 	new Resolution { width = 600, height = 600, refreshRate = 165 },
			// 	new Resolution { width = 600, height = 600, refreshRate = 60 },
			// 	new Resolution { width = 100, height = 100, refreshRate = 30 },
			// 	new Resolution { width = 100, height = 100, refreshRate = 15 },
			// 	new Resolution { width = 100, height = 100, refreshRate = 128 },
			// 	new Resolution { width = 150, height = 150, refreshRate = 30 },
			// };

			//TODO: Optimize this. Don't use LINQ.
			uniqueResolutions = allResolutions.OrderBy(x => x.width).ThenByDescending(x => x.refreshRate).Distinct(resolutionComparer).ToArray();

			return uniqueResolutions;
		}

		public void SetDropdownValue(int index)
		{
			GetUniqueResolutions();

			Value = uniqueResolutions[index];
		}

		public int GetDropdownValue()
		{
			GetUniqueResolutions();

			int index = 0;

			Resolution currentValue = Value;

			for (int i = 0; i < uniqueResolutions.Length; i++)
			{
				if (uniqueResolutions[i].width == currentValue.width && uniqueResolutions[i].height == currentValue.height)
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

			List<(string text, Sprite icon)> values = new List<(string text, Sprite icon)>(uniqueResolutions.Length);

			for (int i = 0; i < uniqueResolutions.Length; i++)
			{
				values.Add((string.Format(resolutionFormat, uniqueResolutions[i].width.ToString(), uniqueResolutions[i].height.ToString(), uniqueResolutions[i].refreshRate.ToString()), null));
			}
			
			values.TrimExcess();

			return values;
		}

		private class ResolutionComparer : IEqualityComparer<Resolution>
		{
			public bool Equals(Resolution x, Resolution y)
			{
				return x.width == y.width && x.height == y.height;
			}

			public int GetHashCode(Resolution obj)
			{
				unchecked
				{
					return (obj.width * 397) ^ obj.height;
				}
			}
		}
	}
}