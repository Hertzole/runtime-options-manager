using System;
using Hertzole.OptionsManager.Extensions;
using UnityEngine;

namespace Hertzole.OptionsManager
{
#if UNITY_EDITOR
	[CreateAssetMenu(fileName = "New Int Setting", menuName = "Hertzole/Settings/Int Setting")]
#endif
	public class IntSetting : Setting<int>, IMinMaxInt, ICanHaveSlider
	{
		[SerializeField]
		private ToggleableInt minValue = new ToggleableInt(false, 0);
		[SerializeField]
		private ToggleableInt maxValue = new ToggleableInt(false, 0);
		[SerializeField]
		private bool enableSlider = default;
		
		public bool EnableSlider { get { return enableSlider; } set { enableSlider = value; } }
		public bool WholeSliderNumbers { get { return true; } }

		public ToggleableInt MinValue { get { return minValue; } set { minValue = value; } }
		public ToggleableInt MaxValue { get { return maxValue; } set { maxValue = value; } }

		protected override void SetValue(int newValue)
		{
			newValue = this.Clamp(newValue);

			base.SetValue(newValue);
		}

		protected override int TryConvertValue(object newValue)
		{
			return Convert.ToInt32(newValue);
		}
	}
}