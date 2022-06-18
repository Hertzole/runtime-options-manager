using System;
using UnityEngine;

namespace Hertzole.RuntimeOptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Float Setting", menuName = "Hertzole/Settings/Float Setting")]
#endif
	public class FloatSetting : Setting<float>, IMinMaxFloat, ICanHaveSlider
	{
		[SerializeField]
		private bool hasMinValue = default;
		[SerializeField]
		private float minValue = default;
		[SerializeField]
		private bool hasMaxValue = default;
		[SerializeField]
		private float maxValue = default;
		[SerializeField] 
		private bool enableSlider = default;
		[SerializeField] 
		private bool wholeSliderNumbers = false;

		public bool HasMinValue { get { return hasMinValue; } set { hasMinValue = value; } }
		public bool HasMaxValue { get { return hasMaxValue; } set { hasMaxValue = value; } }

		public float MinValue { get { return minValue; } set { minValue = value; } }
		public float MaxValue { get { return maxValue; } set { maxValue = value; } }
		public bool EnableSlider { get { return enableSlider; } set { enableSlider = value; } }
		public bool WholeSliderNumbers { get { return wholeSliderNumbers; } set { wholeSliderNumbers = value; } }

		protected override void SetValue(float newValue)
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

		protected override float TryConvertValue(object newValue)
		{
			return Convert.ToSingle(newValue);
		}
	}
}