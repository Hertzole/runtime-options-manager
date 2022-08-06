using System;
using Hertzole.OptionsManager.Extensions;
using UnityEngine;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Float Setting", menuName = "Hertzole/Settings/Float Setting")]
#endif
	public class FloatSetting : Setting<float>, IMinMaxFloat, ICanHaveSlider
	{
		[SerializeField] 
		private ToggleableFloat minValue = new ToggleableFloat(false, 0);
		[SerializeField]
		private ToggleableFloat maxValue = new ToggleableFloat(false, 0);
		[SerializeField] 
		private bool enableSlider = default;
		[SerializeField] 
		private bool wholeSliderNumbers = false;

		public ToggleableFloat MinValue { get { return minValue; } set { minValue = value; } }
		public ToggleableFloat MaxValue { get { return maxValue; } set { maxValue = value; } }
		
		public bool EnableSlider { get { return enableSlider; } set { enableSlider = value; } }
		public bool WholeSliderNumbers { get { return wholeSliderNumbers; } set { wholeSliderNumbers = value; } }

		protected override void SetValue(float newValue)
		{
			newValue = this.Clamp(newValue);

			base.SetValue(newValue);
		}

		protected override float TryConvertValue(object newValue)
		{
			return Convert.ToSingle(newValue);
		}
	}
}