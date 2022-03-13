using System;
using UnityEngine;

namespace Hertzole.Settings
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Int Setting", menuName = "Hertzole/Settings/Int Setting")]
#endif
	public class IntSetting : Setting<int>, IMinMaxInt, ICanHaveSlider
	{
		[SerializeField]
		private bool hasMinValue = default;
		[SerializeField]
		private int minValue = default;
		[SerializeField]
		private bool hasMaxValue = default;
		[SerializeField]
		private int maxValue = default;
		[SerializeField]
		private bool enableSlider = default;
		
		public bool EnableSlider { get { return enableSlider; } set { enableSlider = value; } }
		public bool WholeSliderNumbers { get { return true; } }

		public bool HasMinValue { get { return hasMinValue; } set { hasMinValue = value; } }
		public bool HasMaxValue { get { return hasMaxValue; } set { hasMaxValue = value; } }

		public int MinValue { get { return minValue; } set { minValue = value; } }
		public int MaxValue { get { return maxValue; } set { maxValue = value; } }

		protected override void SetValue(int newValue)
		{
			if (hasMinValue && newValue < minValue)
			{
				newValue = minValue;
			}
			else if (hasMaxValue && newValue > maxValue)
			{
				newValue = maxValue;
			}

			base.SetValue(newValue);
		}

		protected override int TryConvertValue(object newValue)
		{
			return Convert.ToInt32(newValue);
		}
	}
}